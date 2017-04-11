using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System;
using SampleStoreLibrary.Models;
using Microsoft.WindowsAzure;
using Microsoft.Azure;

namespace SampleStore_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {

        private CloudQueue videoQueue;
        private CloudBlobContainer videoBlobContainer;
        private String fullInPath;
        private String fullOutPath;
        private String fileTitle;
        Stopwatch stopWatch = new Stopwatch();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public static String GetExePath()
        {
            //returns the full path of the executeable file in the worker role
            return Path.Combine(Environment.GetEnvironmentVariable("RoleRoot") + @"\", @"approot\ffmpeg.exe");
        }

        public static String GetExeArgs(String inPath, String outPath, int seconds = 10)
        {
            //returns command line arguments
            return "-t " + seconds + " -i " + inPath + " -acodec copy " + outPath;
        }

        public static String GetLocalStoragePath()
        {
            //returns the full path of the local storage
            LocalResource l = RoleEnvironment.GetLocalResource("LocalVideoStore");
            return string.Format(l.RootPath);
        }

        public static String GetInstanceIndex()
        {
            //returns the instance's index
            string instanceId = RoleEnvironment.CurrentRoleInstance.Id;
            return instanceId.Substring(instanceId.LastIndexOf("_") + 1);
        }

        //Call the FFMpeg.exe via a process to Shorten it to a sample.
        private bool ConvertSample(int seconds = 30)
        {
            bool success = false;

            try
            {
                Process proc = new Process();
                //set exe's location
                proc.StartInfo.FileName = GetExePath();
                //set command line args
                proc.StartInfo.Arguments = GetExeArgs(fullInPath, fullOutPath, seconds);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.ErrorDialog = false;

                //execute code
                proc.Start();
                proc.WaitForExit();
                success = true;

                Log("It worked! A video sample has been created!");

            }
            catch (Exception e)
            {
                Trace.TraceError(e.StackTrace);
            }

            return success;
        }

        public override void Run()
        {
            Log("Shortener_WorkerRole is running");

            CloudQueueMessage msg = null;

            //Run an infinite loop to check for new Queue messages.
            while (true)
            {
                try
                {
                    //poll for new message
                    msg = this.videoQueue.GetMessage();
                    if (msg != null)
                    {
                        //Check here for file name or Sample ID.
                        int id;
                        if (Int32.TryParse(msg.AsString, out id))
                        {
                            //Is a Sample ID, message came from ApiWebRole.
                            //Process the message.
                            ProcessQueueMessageFromApi(msg, id);
                        }
                        else
                        {
                            //Is a file name, message came from WebRole.
                            //Process the message.
                            ProcessQueueMessage(msg);
                        }
                    }
                    else
                    {
                        //Wait one second before checking for new queue messages.
                        Thread.Sleep(1000);
                    }
                }
                catch (StorageException e)
                {
                    Trace.TraceError("Message creating a Storage Exception: '{0}'", e.Message);
                    //remove message from queue if it fails more than five times
                    if (msg != null && msg.DequeueCount > 5)
                    {
                        this.videoQueue.DeleteMessage(msg);
                        Trace.TraceError("Deleting poison queue item: '{0}'", msg.AsString);
                    }
                    //Wait 5 seconds before checking for new messages after an Exception occured.
                    Thread.Sleep(5000);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections.
            ServicePointManager.DefaultConnectionLimit = 12;

            // Open storage account using credentials set in role properties and embedded in .cscfg file.
            var storageAccount = CloudStorageAccount.Parse
                (RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            //Log("Exe located at: " + GetExePath());
            Log("Creating videos blob container");

            //Get reference to the Azure Container.
            var blobClient = storageAccount.CreateCloudBlobClient();
            videoBlobContainer = blobClient.GetContainerReference("videos");

            //Create the Container if it doesn't already exits.
            if (videoBlobContainer.CreateIfNotExists())
            {
                videoBlobContainer.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
            }

            //Get a reference to the Azure Cloud Queue for use later, or create it if it doesn't exist.
            Log("Creating video queue");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            videoQueue = queueClient.GetQueueReference("videoqueue");
            videoQueue.CreateIfNotExists();

            Log("Storage initialized");
            Log("localStorage path: " + GetLocalStoragePath());
            return base.OnStart();
        }

        private void ProcessQueueMessageFromApi(CloudQueueMessage msg, int id)
        {
            //Get connection to the DB via the Samples Context.
            var dbConnString = CloudConfigurationManager.GetSetting("ShortenerDbConnectionString");
            SamplesContext db = new SamplesContext(dbConnString);

            //Find the record with the ID that was taken from the Message.
            var sample = db.Samples.Find(id);

            Log("ID from queue is " + id);
            Log("CloudQueueMessage is " + msg.AsString);

            Log("File name from DB for ID " + id + " is: " + sample.Title);

            //Store the Blob path as a local variable
            string path = sample.Mp4Blob;

            //get input blob
            CloudBlockBlob inputBlob = videoBlobContainer.GetBlockBlobReference(path);

            //make folder for blob to be downloaded into
            string folder = path.Split('\\')[0];
            Directory.CreateDirectory(GetLocalStoragePath() + @"\" + folder);

            //download file to local storage
            Log("Downloading blob to local storage...");
            videoBlobContainer.GetBlockBlobReference(path).DownloadToFile(GetLocalStoragePath() + path, FileMode.Create);
            Log("Done downloading");

            //get file's current location
            fullInPath = GetLocalStoragePath() + path;

            //new file name
            string videoName = Path.GetFileNameWithoutExtension(inputBlob.Name) + "sample.mp4";
            Log("New file name: " + videoName);

            //get and make directory for file output
            fullOutPath = GetLocalStoragePath() + @"out\" + videoName;
            CloudBlockBlob outputBlob = this.videoBlobContainer.GetBlockBlobReference(@"out\" + videoName);
            System.IO.Directory.CreateDirectory(GetLocalStoragePath() + @"out\");

            //shorten the video to 10s
            Log("Shortening MP4 to 10s.");
            stopWatch.Start();

            ConvertSample(10);

            stopWatch.Stop();
            Log("Took " + stopWatch.ElapsedMilliseconds + " ms to shorten the mp4 video.");
            stopWatch.Reset();

            //set content type to mp4
            outputBlob.Properties.ContentType = "video/mp4";

            //set id3 tags
            Log("Setting ID3 tags.");
            TagLib.File tagFile = TagLib.File.Create(fullOutPath);


            tagFile.Tag.Comment = "Shortened on WorkerRole Instance " + GetInstanceIndex();
            tagFile.Tag.Conductor = "Edmond Hobeika";
            //Check that title tag isn't null
            fileTitle = tagFile.Tag.Title ?? "File has no original Title Tag";
            tagFile.Save();

            LogMP4Metadata(tagFile);


            //upload blob  from local storage to container
            Log("Returning mp4 to the blob container.");
            using (var fileStream = File.OpenRead(fullOutPath))
            {
                outputBlob.UploadFromStream(fileStream);
            }

            //Add metadata to blob
            Log("Adding metadata to the blob.");
            outputBlob.FetchAttributes();
            outputBlob.Metadata["Title"] = fileTitle;
            outputBlob.Metadata["InstanceNo"] = GetInstanceIndex();
            outputBlob.SetMetadata();

            //Add the SampleMP4Date to the DB record.
            sample.SampleMp4Blob = @"out\" + videoName;
            sample.CreatedDate = DateTime.Now;

            //Save changes made to the record.
            db.SaveChanges();

            //Print blob metadata to console
            Log("Blob's metadata: ");
            foreach (var item in outputBlob.Metadata)
            {
                Log("   " + item.Key + ": " + item.Value);
            }

            //remove message from queue
            Log("Removing message from the queue.");
            videoQueue.DeleteMessage(msg);

            //remove initial blob
            Log("Deleting the input blob.");
            inputBlob.Delete();

            //remove files from local storage
            Log("Deleting files from local storage.");
            File.Delete(fullInPath);
            File.Delete(fullOutPath);
        }

        private void ProcessQueueMessage(CloudQueueMessage msg)
        {
            //get file's path from message queue
            string path = msg.AsString;

            //get input blob
            CloudBlockBlob inputBlob = videoBlobContainer.GetBlockBlobReference(path);

            //make folder for blob to be downloaded into
            string folder = path.Split('\\')[0];
            System.IO.Directory.CreateDirectory(GetLocalStoragePath() + @"\" + folder);

            //download file to local storage
            Log("Downloading blob to local storage...");
            videoBlobContainer.GetBlockBlobReference(path).DownloadToFile(GetLocalStoragePath() + path, FileMode.Create);
            Log("Done downloading");

            //get file's current location
            fullInPath = GetLocalStoragePath() + path;

            //new file name
            string videoName = Path.GetFileNameWithoutExtension(inputBlob.Name) + "sample.mp4";
            Log("New file name: " + videoName);

            //get and make directory for file output
            fullOutPath = GetLocalStoragePath() + @"out\" + videoName;
            CloudBlockBlob outputBlob = this.videoBlobContainer.GetBlockBlobReference(@"out\" + videoName);
            System.IO.Directory.CreateDirectory(GetLocalStoragePath() + @"out\");

            //shorten the video to 10s
            Log("Shortening MP4 to 10s.");
            stopWatch.Start();

            ConvertSample(10);

            stopWatch.Stop();
            Log("Took " + stopWatch.ElapsedMilliseconds + " ms to shorten mp4 video.");
            stopWatch.Reset();

            //set content type to mp4
            outputBlob.Properties.ContentType = "video/mp4";

            //set id3 tags
            Log("Setting ID3 tags.");
            TagLib.File tagFile = TagLib.File.Create(fullOutPath);


            tagFile.Tag.Comment = "Shortened on WorkerRole Instance " + GetInstanceIndex();
            tagFile.Tag.Conductor = "Edmond Hobeika";
            //Check that title tag isn't null
            fileTitle = tagFile.Tag.Title ?? "File has no original Title Tag";
            tagFile.Save();

            LogMP4Metadata(tagFile);


            //upload blob  from local storage to container
            Log("Returning mp4 to the blob container.");
            using (var fileStream = File.OpenRead(fullOutPath))
            {
                outputBlob.UploadFromStream(fileStream);
            }

            //Add metadata to blob
            Log("Adding metadata to the blob.");
            outputBlob.FetchAttributes();
            outputBlob.Metadata["Title"] = fileTitle;
            outputBlob.Metadata["InstanceNo"] = GetInstanceIndex();
            outputBlob.SetMetadata();

            //Print blob metadata to console
            Log("Blob's metadata: ");
            foreach (var item in outputBlob.Metadata)
            {
                Log("   " + item.Key + ": " + item.Value);
            }

            //remove message from queue
            Log("Removing message from the queue.");
            videoQueue.DeleteMessage(msg);

            //remove initial blob
            Log("Deleting the input blob.");
            inputBlob.Delete();

            //remove files from local storage
            Log("Deleting files from local storage.");
            File.Delete(fullInPath);
            File.Delete(fullOutPath);
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SampleStore_WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SampleStore_WorkerRole has stopped");
        }

        //Log data that is taken from the File's Metadata.
        protected void LogMP4Metadata(TagLib.File file)
        {

            Log("File's metadata:");

            Log("  Title: " + file.Tag.Title);
            var artist = (file.Tag.AlbumArtists.Length > 0) ? file.Tag.AlbumArtists[0] : "";
            Log("  Artist: " + artist);
            Log("  Album: " + file.Tag.Album);
            Log("  Year: " + file.Tag.Year);
            var genre = (file.Tag.Genres.Length > 0) ? file.Tag.Genres[0] : "";
            Log("  Genre: " + genre);
            Log("  Comment: " + file.Tag.Comment);

        }

        //Short-hand method to write to Azure Compute Emulator's console.
        protected void Log(String msg)
        {
            Trace.TraceInformation(msg);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
