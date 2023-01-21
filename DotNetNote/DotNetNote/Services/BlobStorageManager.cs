using DotNetNote.Settings;
using HeyRed.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DotNetNoteCore.Services;

public class BlobStorageManager : IStorageManager
{
    private const string PLACEHOLDER = "directory_placeholder.txt";
    private readonly IHttpContextAccessor contextAccessor;
    private readonly AppKeyConfig appKeyConfig;
    private readonly ILogger logger;

    public BlobStorageManager(
        IHttpContextAccessor contextAccessor,
        IOptions<AppKeyConfig> appKeyConfig,
        ILoggerFactory loggerFactory)
    {
        this.contextAccessor = contextAccessor;
        this.appKeyConfig = appKeyConfig.Value;
        this.logger = loggerFactory.CreateLogger<BlobStorageManager>();
    }

    public async Task<bool> CreateDirectory(string folderPath)
    {
        try
        {
            var container = await this.GetBlobContainerAsync("");
            if (container != null)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(folderPath + "/" + PLACEHOLDER);
                await blockBlob.UploadTextAsync("placeholder");

                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string fileName, string folderPath)
    {
        try
        {
            var container = await this.GetBlobContainerAsync("");
            if (container != null)
            {
                CloudBlobDirectory blobDirectory = container.GetDirectoryReference(folderPath);
                CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(fileName);

                if (blockBlob != null)
                {
                    return await blockBlob.DeleteIfExistsAsync();
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<byte[]> DownloadAsync(string fileName, string folderPath)
    {
        try
        {
            var container = await this.GetBlobContainerAsync("");
            if (container != null)
            {
                CloudBlobDirectory blobDirectory = container.GetDirectoryReference(folderPath);
                CloudBlockBlob blockBlob = this.GetBlockBlobAsync(fileName, folderPath, container);

                if (blockBlob != null)
                {
                    await blockBlob.FetchAttributesAsync();
                    var bytes = new byte[blockBlob.Properties.Length];
                    await blockBlob.DownloadToByteArrayAsync(bytes, 0);
                    return bytes;
                }

                return null;
            }

            return null;
        }
        catch (Exception ex)
        {
            string test = ex.Message;
            test += "test";
            return null;
        }
    }

    public async Task<string> DownloadPathAsync(string fileName, string folderPath)
    {
        try
        {
            var container = await this.GetBlobContainerAsync("");
            if (container != null)
            {
                CloudBlobDirectory blobDirectory = container.GetDirectoryReference(folderPath);
                CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(fileName);

                if (blockBlob != null)
                {
                    var sasConstraints = new SharedAccessBlobPolicy();
                    sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
                    sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5);
                    sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

                    var sasBlobToken = blockBlob.GetSharedAccessSignature(sasConstraints);
                    return blockBlob.Uri + sasBlobToken;
                }

                return null;
            }

            return null;
        }
        catch (Exception ex)
        {
            string test = ex.Message;
            test += "test";
            return null;
        }
    }

    public async Task<MemoryStream> DownloadToMemoryStreamAsync(string fileName, string folderPath)
    {
        try
        {
            var container = await this.GetBlobContainerAsync("");
            if (container != null)
            {
                CloudBlobDirectory blobDirectory = container.GetDirectoryReference(folderPath);
                CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(fileName);

                if (blockBlob != null)
                {
                    await blockBlob.FetchAttributesAsync();
                    var memoryStream = new MemoryStream();
                    await blockBlob.DownloadToStreamAsync(memoryStream);
                    return memoryStream;
                }

                return null;
            }

            return null;
        }
        catch (Exception ex)
        {
            string test = ex.Message;
            test += "test";
            return null;
        }
    }

    public string GetFolderPath(string ownerType, long ownerID, string fileType) => ownerType + "/" + ownerID + "/" + fileType;

    public string GetFolderPath(string ownerType, string ownerID, string fileType)
    {
        return ownerType + "/" + ownerID + "/" + fileType;
    }

    public async Task<IDictionary<string, string>> GetMetadataAsync(string fileName, string folderPath)
    {
        try
        {
            var container = await this.GetBlobContainerAsync("");
            if (container != null)
            {
                CloudBlobDirectory blobDirectory = container.GetDirectoryReference(folderPath);
                CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(fileName);

                if (blockBlob != null)
                {
                    await blockBlob.FetchAttributesAsync();
                    return blockBlob.Metadata;
                }

                return null;
            }

            return null;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<string> UploadAsync(byte[] bytes, string fileName, string folderPath, bool overwrite)
    {
        var container = await this.GetBlobContainerAsync("");
        return await this.UploadToContainer(bytes, fileName, folderPath, overwrite, container);
    }

    public async Task<string> UploadAsync(byte[] bytes, string fileName, string folderPath, bool overwrite, string accountID)
    {
        var container = await this.GetBlobContainerByAccountIDAsync(accountID);
        return await this.UploadToContainer(bytes, fileName, folderPath, overwrite, container);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string folderPath, bool overwrite)
    {
        var container = await this.GetBlobContainerAsync("");
        return await this.UploadToContainer(stream, fileName, folderPath, overwrite, container);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string folderPath, bool overwrite, string accountID)
    {
        var container = await this.GetBlobContainerByAccountIDAsync(accountID);
        return await this.UploadToContainer(stream, fileName, folderPath, overwrite, container);
    }

    private CloudBlockBlob GetBlockBlobAsync(string fileName, string folderPath, CloudBlobContainer container)
    {
        var blobName = fileName;
        if (!string.IsNullOrEmpty(folderPath))
        {
            if (folderPath.Substring(folderPath.Length - 1).Equals("/"))
            {
                blobName = folderPath + fileName;
            }
            else
            {
                blobName = folderPath + "/" + fileName;
            }
        }

        return container.GetBlockBlobReference(blobName);
    }

    private async Task<CloudBlobContainer> GetBlobContainerAsync(string email)
    {
        return await this.GetBlobContainerByTenantAsync(email);
    }

    private async Task<CloudBlobContainer> GetBlobContainerByAccountIDAsync(string accountID)
    {
        return await this.GetBlobContainerByTenantAsync(accountID);
    }

    private async Task<CloudBlobContainer> GetBlobContainerByTenantAsync(string email)
    {
        CloudStorageAccount storageAccount = new CloudStorageAccount(
               new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                   this.appKeyConfig.AzureStorageAccount,
                   this.appKeyConfig.AzureStorageAccessKey), true);

        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        var containerName = appKeyConfig.AzureStorageContainer;
        var container = blobClient.GetContainerReference(containerName);
        await container.CreateIfNotExistsAsync();

        return container;
    }

    private async Task<string> GetUniqueFileNameAsync(CloudBlockBlob blob, string folderPath, string fileName, long depth = 1)
    {
        if (await blob.ExistsAsync())
        {
            var blobName = folderPath + "/" + Path.GetFileNameWithoutExtension(fileName) + " (" + depth + ")" + Path.GetExtension(fileName);
            var newBlob = blob.Container.GetBlockBlobReference(blobName);
            return await this.GetUniqueFileNameAsync(newBlob, folderPath, fileName, depth + 1);
        }
        else
        {
            // if original fileName is unique, return
            if (depth == 1)
            {
                return fileName;
            }

            return Path.GetFileNameWithoutExtension(fileName) + " (" + (depth - 1) + ")" + Path.GetExtension(fileName);
        }
    }

    private async Task<bool> SetMetadata(CloudBlockBlob blockBlob, string fileName)
    {
        try
        {
            Task[] tasks = new Task[2];

            var contentType = MimeTypesMap.GetMimeType(fileName);
            blockBlob.Properties.ContentType = contentType;
            tasks[0] = blockBlob.SetPropertiesAsync();

            blockBlob.Metadata["FileName"] = fileName;
            blockBlob.Metadata["ContentType"] = contentType;
            tasks[1] = blockBlob.SetMetadataAsync();

            await Task.WhenAll(tasks);

            return true;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return false;
        }
    }

    private async Task<string> UploadToContainer(byte[] bytes, string fileName, string folderPath, bool overwrite, CloudBlobContainer container)
    {
        try
        {
            CloudBlockBlob blockBlob = this.GetBlockBlobAsync(fileName, folderPath, container);
            if (!overwrite)
            {
                fileName = await this.GetUniqueFileNameAsync(blockBlob, folderPath, fileName);
                blockBlob = this.GetBlockBlobAsync(fileName, folderPath, container);
            }

            await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

            return fileName;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return string.Empty;
        }
    }

    private async Task<string> UploadToContainer(Stream stream, string fileName, string folderPath, bool overwrite, CloudBlobContainer container)
    {
        try
        {
            CloudBlockBlob blockBlob = this.GetBlockBlobAsync(fileName, folderPath, container);
            if (!overwrite)
            {
                fileName = await this.GetUniqueFileNameAsync(blockBlob, folderPath, fileName);
                blockBlob = this.GetBlockBlobAsync(fileName, folderPath, container);
            }

            stream.Seek(0, SeekOrigin.Begin);

            await blockBlob.UploadFromStreamAsync(stream);

            await this.SetMetadata(blockBlob, fileName);

            return fileName;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return string.Empty;
        }
    }
}
