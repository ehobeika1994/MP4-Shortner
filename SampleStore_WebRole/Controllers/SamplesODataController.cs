using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using SampleStoreLibrary.Models;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;

namespace SampleStore_WebRole.Controllers
{
    public class SamplesODataController : ODataController
    {
        private SamplesContext db = new SamplesContext();
        private static CloudBlobClient blobClient;
        private static CloudQueueClient queueStorage;

        private static bool created = false;
        private static object @lock = new Object();

        //Make container and queue objects if they don't exist.
        private void MakeContainerAndQueue()
        {
            if (created)
                return;
            lock (@lock)
            {
                if (created)
                {
                    return;
                }

                try
                {

                    var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

                    blobClient = storageAccount.CreateCloudBlobClient();

                    CloudBlobContainer container = blobClient.GetContainerReference("videos");

                    container.CreateIfNotExists();

                    //var permissions = container.GetPermissions();
                    //permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    //container.SetPermissions(permissions);

                    queueStorage = storageAccount.CreateCloudQueueClient();

                    CloudQueue queue = queueStorage.GetQueueReference("videoqueue");

                    queue.CreateIfNotExists();
                }
                catch (WebException)
                {

                    throw new WebException("The Windows Azure storage services cannot be contacted " +
                         "via the current account configuration or the local development storage emulator is not running. ");
                }

                created = true;
            }
        }

        private CloudBlobContainer GetVideosContainer()
        {
            //returns the container where the video files are stored
            MakeContainerAndQueue();
            return blobClient.GetContainerReference("videos");
        }

        private CloudQueue GetVideoQueue()
        {
            //returns the CloudQueue where video paths are stored temp
            MakeContainerAndQueue();
            return queueStorage.GetQueueReference("videosqueue");
        }

        // GET: odata/SamplesOData(5)
        [EnableQuery]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage res = new HttpResponseMessage();

            // check if there is records in the database with a specific ID
            if (db.Samples.Any(o => o.SampleID == id))
            {
                var sampleId = db.Samples.Find(id);

                if (sampleId.SampleMp4Blob == null || sampleId.SampleMp4Blob == "")
                {
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Content = new StringContent("No content found for that video sample");
                    return res;
                }

                //Get blob from the container via its SampleMp4Blob attribute.
                CloudBlockBlob blob = GetVideosContainer().GetBlockBlobReference(sampleId.SampleMp4Blob);

                Stream blobStream = blob.OpenRead();

                //Set the content to the blob's stream.
                res.Content = new StreamContent(blobStream);

                //Set HTTP headers.
                res.Content.Headers.ContentLength = blob.Properties.Length;
                res.Content.Headers.ContentType = new
                System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4");
                res.Content.Headers.ContentDisposition = new
                System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = blob.Name,
                    Size = blob.Properties.Length
                };

                return res;
            }
            else
            {
                //Sample not found
                res.Content = new StringContent("Video Sample with id " + id + " doesn't exist, so you can't download it");
                res.StatusCode = HttpStatusCode.NotFound;
            }

                return res;
        }

       // PUT: odata/SamplesOData(5)
       public HttpResponseMessage Put(int id)
        {
            HttpResponseMessage res = new HttpResponseMessage();

            if (db.Samples.Any(o => o.SampleID == id))
            {
                // Sample is in the database
                var sampleId = db.Samples.Find(id);

                res.StatusCode = HttpStatusCode.OK;

                if (sampleId.SampleMp4Blob != null)
                {

                    //remove old blob first
                    if (GetVideosContainer().GetBlockBlobReference(sampleId.SampleMp4Blob).Exists())
                    {
                        CloudBlockBlob oldBlob = GetVideosContainer().GetBlockBlobReference(sampleId.SampleMp4Blob);
                        oldBlob.Delete();
                    }

                }

                //Set the sample url
                var baseUrl = Request.RequestUri.Host + ":" + (Int32.Parse(Request.RequestUri.GetComponents(UriComponents.StrongPort, UriFormat.SafeUnescaped)));

                String sampleMp4URL = "http://" + baseUrl.ToString() + "/api/data/" + id;
                sampleId.SampleMp4URL = sampleMp4URL;

                var request = HttpContext.Current.Request;

                //rename file

                var newName = string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ".mp4");

                //set initial folder name to 'in'
                String path = @"in\" + newName;

                //get the blob as a variable
                var blob = GetVideosContainer().GetBlockBlobReference(path);

                //upload blob
                blob.UploadFromStream(request.InputStream);

                //Store the mp4 blob uri in DB
                sampleId.Mp4Blob = path;

                db.SaveChanges();

                //db.Entry(sample).State = System.Data.Entity.EntityState.Detached;

                //Add sample id to the queue
                GetVideoQueue().AddMessage(new CloudQueueMessage(id.ToString()));


                res.Content = new StringContent("Uploaded data to container via put for id: " + id);

            }
            else
            {
                //Sample not found
                res.Content = new StringContent("Video Sample with id " + id + " doesn't exist, so you can't put that data here.");
                res.StatusCode = HttpStatusCode.NotFound;
            }

            return res;

        }
    }
}
