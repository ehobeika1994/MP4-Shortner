using SampleStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace SampleStoreClient
{
    // Based on: http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
    // Make sure that Nuget Default Project is set to this project before executing:
    //      Install-Package Microsoft.AspNet.WebApi.Client
    class Program
    {
        //Initialise new Http Client.
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {  
                //Print App Name.
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}", " Video Sample Test Client\n Edmond Hobeika - S1238520\n");

                //Uncomment the appropriate Base Address for where the Api is running.
                client.BaseAddress = new Uri("http://localhost:50217/");

                //Clear existing headers.
                client.DefaultRequestHeaders.Accept.Clear();
                //Request JSON instead of XML from the API.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("Checking connection to API...");

                //Send a Get request to "/" to see if the API is online.
                try
                {
                    HttpResponseMessage response = await client.GetAsync("/");
                    Console.WriteLine();
                }
                catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
                {
                    //Exit application if API is offline.
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not reach the API. \n" + ex.Message + "\n\nExiting Application");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                await ShowMenu();
            }
        }

        private static async Task ShowMenu()
        {
            //Display Menu to the user.
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("1.  GET all sample videos");
            Console.WriteLine("2.  GET specific sample video");
            Console.WriteLine("3.  POST new sample video");
            Console.WriteLine("4.  DELETE a sample video");
            Console.WriteLine("5.  PUT information for existing sample video");
            Console.WriteLine("6.  PUT blob data for existing sample video");
            Console.WriteLine("7.  GET blob data for existing sample video");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0.  Exit the application.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nSelect an option by entering a number..");
            Console.WriteLine();

            //Get user's choice
            int input = 0;
            bool inp = int.TryParse(Console.ReadLine(), out input);
            
            if (inp)
            {
                bool valid;
                Console.WriteLine();
                //Choose method to run in response to the user's input
                switch (input)
                {
                    case 0:

                        //Close application
                        Environment.Exit(0);
                        break;

                    case 1:

                        //GET all video samples
                        Console.WriteLine("Getting data on all video samples.");
                        Console.WriteLine();
                        Console.WriteLine("Loading content.........");
                        try
                        {
                            await GetSampleAsync();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Something's wrong!");
                            ShowError(e);
                            await ReturnToMenu();
                        }
                        break;

                    case 2:
                        //Get a specific sample
                        Console.WriteLine("Enter the ID of the Sample Video you want information on");
                        Console.WriteLine();
                        input = 0;
                        valid = int.TryParse(Console.ReadLine(), out input);
                        if (valid)
                        {
                            try
                            {
                                await GetOne(input);
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine("Something's wrong here!" + e);
                                ShowError(e);
                                await ReturnToMenu();
                            }
                        }
                        else
                        {
                            await ShowMenu();
                        }


                        break;

                    case 3:
                        //Post new sample.
                        Console.WriteLine("Posting hard coded data.");
                        Console.WriteLine();
                        try
                        {
                            await Post();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Something's wrong!" + e);
                            await ReturnToMenu();
                        }

                        break;

                    case 4:
                        //Delete a sample with an ID that the user specifies.
                        Console.WriteLine("Enter the ID of the Sample Video you want to delete");
                        Console.WriteLine();

                        input = 0;
                        valid = int.TryParse(Console.ReadLine(), out input);
                        if (valid)
                        {
                            try
                            {
                                await Delete(input);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Something's wrong!" + e);
                                await ReturnToMenu();
                            }  
                        }
                        else
                        {
                            await ShowMenu();
                        }

                        break;

                    case 5:
                        //Put data to the api and overwrite existing data.
                        Console.WriteLine("Enter the ID of the sample video you want overwrite");
                        Console.WriteLine();
                        input = 0;
                        valid = int.TryParse(Console.ReadLine(), out input);
                        if (valid)
                        {
                            try
                            {
                                await Put(input);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Something's wrong!" + e);
                                await ReturnToMenu();
                            }
                            
                        }
                        else
                        {
                            await ShowMenu();
                        }

                        break;

                    case 6:
                        //Put an MP4's data to the data api
                        Console.WriteLine("Enter the ID of the video blob you want to put to");
                        Console.WriteLine();

                        input = 0;
                        valid = int.TryParse(Console.ReadLine(), out input);
                        if (valid)
                        {
                            try
                            {
                                await PutBlob(input);
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine("Something's wrong!" + e);
                                ShowError(e);
                                await ReturnToMenu();
                            }
                           
                        }
                        else
                        {
                            await ShowMenu();
                        }

                        break;

                    case 7:
                        //Download data from a blob.
                        Console.WriteLine("Enter the ID of the video blob you want to get");
                        Console.WriteLine();

                        input = 0;
                        valid = int.TryParse(Console.ReadLine(), out input);
                        if (valid)
                        {
                            try
                            {
                                await GetBlob(input);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Something's wrong!" + e);
                                await ReturnToMenu();
                            }
                            
                        }
                        else
                        {
                            await ShowMenu();
                        }

                        break;

                    default:
                        //handle invalid numeric options.
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input integer, try again..");
                        Console.WriteLine();
                        await ShowMenu();
                        break;
                }
            }
            else
            {
                //Handle invalid options.
                Console.WriteLine("Please enter an integer");
                Console.WriteLine();

                await ShowMenu();
            }
        }
   
        static async Task GetSampleAsync()
        {
            //IEnumerable<Sample> samples = null;
            HttpResponseMessage response;
            response = await client.GetAsync("http://localhost:50217/api/samples");
            if(response.IsSuccessStatusCode)
            {
                IEnumerable<Sample> samples = await response.Content.ReadAsAsync<IEnumerable<Sample>>();
                Console.WriteLine("Samples:");

                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", "ID     ", "Title     ", "Genre     ", "MP4 Blob     ", "Sample MP4 Blob     ", "Sample MP4 URL     ", "Date     ");
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", "--     ", "-----     ", "------     ", "----     ", "----     ", "----     ", "----     ");
                foreach (var sample in samples)
                {
                    Console.WriteLine("{0}\t{1}\t${2}\t{3}\t{4}\t{5}\t{6}", sample.SampleID, sample.Title, sample.Genre, sample.Mp4Blob, sample.SampleMp4Blob, sample.SampleMp4URL, sample.CreatedDate);
                }
            }
            //return samples;
            await ReturnToMenu();
        }

        private static async Task GetOne(int id)
        {
            //Get a sample by id
            HttpResponseMessage response;
            response = await client.GetAsync("http://localhost:50217/api/samples/" + id);
            if (response.IsSuccessStatusCode)
            {
                //Convert Json to Sample Object.
                Sample sample = await response.Content.ReadAsAsync<Sample>();

                //Display Sample data on console.
                Console.WriteLine("Sample #:");

                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", "ID     ", "Title     ", "Genre     ", "MP4 Blob     ", "Sample MP4 Blob     ", "Sample MP4 URL     ", "Date     ");
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", "--     ", "-----     ", "------     ", "----     ", "----     ", "----     ", "----     ");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0}\t{1}\t${2}\t{3}\t{4}\t{5}\t{6}", sample.SampleID, sample.Title, sample.Genre, sample.Mp4Blob, sample.SampleMp4Blob, sample.SampleMp4URL, sample.CreatedDate);

            }
            await ReturnToMenu();
        }

        private static async Task Post()
        {
            //Post a new sample
            Random rand = new Random();
            var sample = new Sample() { Title = "Bee 28", Genre = "Adventure 28", Mp4Blob = null, SampleMp4Blob = null, SampleMp4URL = null, CreatedDate = DateTime.Now };
            HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:50217/api/samples", sample);
            if (response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successfully Posted new Sample to the api");
            }

            await ReturnToMenu();
        }

        private static async Task Delete(int id)
        {
            //Send a http delete to the Api with the ID to delete
            HttpResponseMessage response;
            response = await client.DeleteAsync("http://localhost:50217/api/samples/" + id);

            if (response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Deleted sample at id: " + id);
            }

            await ReturnToMenu();
        }

        private static async Task Put(int id)
        {
            //Overwrite information in an existing record by ID
            HttpResponseMessage response;
            response = await client.GetAsync("http://localhost:50217/api/samples/" + id);
            if (response.IsSuccessStatusCode)
            {
                // Snippet from the client code from PUT

                Console.Write("Put sample id: ");
                string sampleId = Console.ReadLine();
                Console.WriteLine();

                string path = "http://localhost:50217/api/samples/" + id;

                // Note that 4 of these fields are null
                // Only the data controller and worker role give these values
                // The client will always send null values for these (in POST as well)
                var newSample = new Sample()
                {
                    SampleID = Int32.Parse(sampleId), // NB - this field is not present in a POST
                    Title = "Horses",
                    Genre = "Wildlife",
                    Mp4Blob = null,
                    SampleMp4Blob = null,
                    SampleMp4URL = null,
                    CreatedDate = DateTime.Now
                };

                response = await client.PutAsJsonAsync(path, newSample);
                response.EnsureSuccessStatusCode();
                Uri newSampleURL = response.Headers.Location;
                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Successfully updated Sample at ID " + id + " via PUT");
                }
            }

            await ReturnToMenu();
        }

        private static async Task PutBlob(int id)
        {
            //Put blob to container over http.

            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Upload\giraffes.mp4");

            using (var stream = File.OpenRead(path))
            {
                //Write file stream to the API.
                HttpResponseMessage res = await client.PutAsync("http://localhost:50217/api/data/" + id, new StreamContent(stream));
                res.EnsureSuccessStatusCode();

                if (res.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PUT file successfully to sample id " + id);
                }

            }

            await ReturnToMenu();
        }

        private static async Task GetBlob(int id)
        {
            //Get blob from container over http

            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Download\Downloaded-File-" + DateTime.Now.Ticks + ".mp4");

            HttpResponseMessage response = await client.GetAsync("http://localhost:50217/api/data/" + id);

            if (response.IsSuccessStatusCode)
            {

                //Save stream to a file.
                byte[] bytes = response.Content.ReadAsByteArrayAsync().Result;
                Console.WriteLine("Downloaded {0} bytes", bytes.Length);
                using (FileStream fileStream = new FileStream(path,
                FileMode.Create, FileAccess.Write))

                using (BinaryWriter binaryFileWriter = new BinaryWriter(fileStream))
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        binaryFileWriter.Write(bytes[i]);
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Downloaded mp4.");

            }

            await ReturnToMenu();

        }

        static void ShowError(Exception ex)
        {
            //Display an error the the user.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An unexpected error occurred.");
            string message = ex.Message;
            if (ex.InnerException != null)
            {
                message += Environment.NewLine + "Inner Exception : " + ex.InnerException.Message;
            }
            Console.WriteLine("Message: {0}", message);
        }

        private static async Task ReturnToMenu()
        {
            //Returns the user to the Menu method.
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.WriteLine();
            await ShowMenu();
        }
    }
}