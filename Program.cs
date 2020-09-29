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

           // Upload blobs to a container:

            // Create a local file in the ./data/ directory for uploading and downloading
            string localPath = "../data/";
            string fileName = "myFile" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);

            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, "Hello, Jamil!");

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            //Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
            Console.WriteLine($"Uploading to Blob Storage:\n\t {blobClient.Uri}\n");

            // Open the file and upload its data
            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();


            // List all blobs in the container

            Console.WriteLine("Blobs Lists");
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }

            //Downloading blobs:

            string downloadFilePath = localFilePath.Replace(".txt", "Download.txt");

            Console.WriteLine($"\nDownloading blob to\n\t{ downloadFilePath }\n");

            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using FileStream downloadFileStream = File.OpenWrite(downloadFilePath);

            await download.Content.CopyToAsync(downloadFileStream);

            downloadFileStream.Close();

            //Deleting a Container:

            Console.WriteLine("Type y to Delete or n to exit:\n");

            bool choise = Console.ReadLine() == "y" ? true : false;

            if (choise)
            {

                await containerClient.DeleteAsync();

                File.Delete(localFilePath);

                File.Delete(downloadFilePath);

                Console.WriteLine("Done!");
            }
            else
                Console.WriteLine("Exiting..");


            Console.ReadKey();
        }
    }
}
