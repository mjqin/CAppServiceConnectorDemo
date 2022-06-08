using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using System.IO;
using webapp_storage_mi.Pages;

namespace webapp_storage_mi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                // Construct the blob container endpoint from the arguments.
                string containerEndpoint = Environment.GetEnvironmentVariable("AZURE_STORAGEBLOB_RESOURCEENDPOINT");

                // Get a credential and create a client object for the blob container.
                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(containerEndpoint),
                                                                                new DefaultAzureCredential());

                var blobContainer = blobServiceClient.GetBlobContainerClient("test");
                blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blob = blobContainer.GetBlobClient("test.txt");

                var response = blob.DownloadAsync().Result;

                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    var myModel = new MyViewModel() { Message = "Failed to read blob!" };
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLineAsync().Result;
                        myModel.Message = line;
                    }
                    return View(myModel);
                }
            }
            catch (Exception e)
            {
                return View(new MyViewModel() { Message = e.Message });
            }
        }
    }
}
