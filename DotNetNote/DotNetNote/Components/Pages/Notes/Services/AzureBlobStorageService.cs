using Azunt.NoteManagement;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System;

namespace Azunt.Web.Components.Pages.Notes.Services
{
    public class AzureBlobStorageService : INoteStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _subFolder;

        public AzureBlobStorageService(IConfiguration config, string subFolder = "notes")
        {
            _subFolder = subFolder;

            var connStr = config["AzureBlobStorage:Default:ConnectionString"];
            var containerName = config["AzureBlobStorage:Default:ContainerName"];

            if (string.IsNullOrWhiteSpace(connStr) || string.IsNullOrWhiteSpace(containerName))
                throw new InvalidOperationException("Azure Blob Storage configuration is missing.");

            _containerClient = new BlobContainerClient(connStr, containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadAsync(Stream stream, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));

            string safeFileName = await GetUniqueFileNameAsync(fileName);
            string blobPath = $"{_subFolder}/{safeFileName}";

            var blobClient = _containerClient.GetBlobClient(blobPath);
            await blobClient.UploadAsync(stream, overwrite: true);

            return safeFileName;
        }

        private async Task<string> GetUniqueFileNameAsync(string originalName)
        {
            string baseName = Path.GetFileNameWithoutExtension(originalName);
            string extension = Path.GetExtension(originalName);
            string newFileName = originalName;
            int count = 1;

            while (await _containerClient.GetBlobClient($"{_subFolder}/{newFileName}").ExistsAsync())
            {
                newFileName = $"{baseName}({count}){extension}";
                count++;
            }

            return newFileName;
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));

            string blobPath = $"{_subFolder}/{fileName}";

            var blobClient = _containerClient.GetBlobClient(blobPath);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException($"Note file not found: {fileName}");

            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public async Task DeleteAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));

            string blobPath = $"{_subFolder}/{fileName}";
            string decodedPath = WebUtility.UrlDecode(blobPath);

            var blobClient = _containerClient.GetBlobClient(decodedPath);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}