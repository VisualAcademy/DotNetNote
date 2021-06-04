using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace DotNetNoteCom.Controllers
{
    public class BlobController : Controller
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0
        private IConfiguration _config;

        public BlobController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// // https://docs.microsoft.com/ko-kr/azure/visual-studio/vs-storage-aspnet5-getting-started-blobs
        /// </summary>
        public async Task<IActionResult> Index()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                 new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                 _config.GetSection("Blob").GetSection("Account").Value,
                 _config.GetSection("Blob").GetSection("AccessKey").Value), true);

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named "my-new-container."
            CloudBlobContainer container = blobClient.GetContainerReference("my-new-container");

            // If "mycontainer" doesn't exist, create it.
            await container.CreateIfNotExistsAsync();

            // Get a reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("test34.txt");

            // Create or overwrite the "myblob" blob with the contents of a local file
            // named "myfile".
            using (var fileStream = System.IO.File.OpenRead(@"C:\Temp\test.txt"))
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            return View();
        }

        /// <summary>
        /// https://developer.telerik.com/products/kendo-ui/file-uploads-azure-asp-angular/
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveFile(IFormFile files) // input 태그의 name
        {
            // Connect to Azure
            // Set the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=celltrionweb;AccountKey=I0IscrCtZ6cCvGRJPXRKsH5iavku70ksdlTFM3WRWLq8+FIZnLJ94nmuh5U3jy33LWZoHf5xAe0T4e0pR62H2A==;EndpointSuffix=core.windows.net");

            // Create a blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container  
            CloudBlobContainer container = blobClient.GetContainerReference("my-new-container");

            // Save file to blob
            // Get a reference to a blob  
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(files.FileName);

            // Create or overwrite the blob with the contents of a local file 
            using (var fileStream = files.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            // Respond with success
            // Respond with success
            //return RedirectToAction(nameof(Index));
            return Json(new
            {
                name = blockBlob.Name,
                uri = blockBlob.Uri,
                size = blockBlob.Properties.Length
            });
        }
    }
}
