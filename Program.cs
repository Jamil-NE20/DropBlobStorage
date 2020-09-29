using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DropBlobStorage
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Azure Blob storage: DropBox\n");
            string connectionString = Environment.GetEnvironmentVariable("Storage_Account");

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            //Create a unique name for the container
            string containerName = "jamilblobs" + Guid.NewGuid().ToString();

            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);


            // Create a local file in the ./data/ directory for uploading and downloading
            string localPath = "C:/Users/Studerande/Desktop/Lexicon/Study information and Problem/Azure/Lab/DropBoxConsole/DropBlopStorage/data/";
            string fileName = "myFile" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);

            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, "Hello, World!");

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            //Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
            Console.WriteLine($"Uploading to Blob Storage:\n\t {blobClient.Uri}\n");

            // Open the file and upload its data
            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
        }
    }
}
