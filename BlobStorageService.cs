using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace webapi.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["AzureBlobStorage:ConnectionString"]);
            _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "courses";
        }

        public async Task<string> UploadCourseFileAsync(IFormFile file, string courseIdPrefix = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null", nameof(file));
            }

            // Get a reference to the container
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Create the container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Create a unique blob name
            string uniqueFileName = courseIdPrefix != null
                ? $"course_{courseIdPrefix}_{Guid.NewGuid()}_{file.FileName}"
                : $"{Guid.NewGuid()}_{file.FileName}";

            // Remove any invalid characters from the filename
            uniqueFileName = string.Join("_", uniqueFileName.Split(Path.GetInvalidFileNameChars()));

            // Get a reference to the blob
            BlobClient blobClient = containerClient.GetBlobClient(uniqueFileName);

            // Upload the file
            using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            // Return the URL of the blob
            return blobClient.Uri.ToString();
        }

        public async Task<List<string>> ListCourseFilesAsync(string prefix = null)
        {
            List<string> blobUrls = new List<string>();
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Create if the container doesn't exist
            await containerClient.CreateIfNotExistsAsync();

            // List all blobs in the container
            var blobs = containerClient.GetBlobsAsync(prefix: prefix);

            await foreach (var blob in blobs)
            {
                blobUrls.Add(containerClient.GetBlobClient(blob.Name).Uri.ToString());
            }

            return blobUrls;
        }
    }
}