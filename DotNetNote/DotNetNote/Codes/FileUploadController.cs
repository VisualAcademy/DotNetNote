using Azure.Storage.Blobs;

namespace VisualAcademy.Codes;

/// <summary>
/// Azure Blob Storage에 파일 업로드 및 다운로드 작업을 처리합니다.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Administrators")]
public class FileUploadController(
    IWebHostEnvironment environment,
    IConfiguration configuration) : ControllerBase
{
    private readonly string _containerName = "files";

    /// <summary>
    /// 로컬 "files" 폴더에서 파일을 읽어 Azure Blob Storage로 업로드합니다.
    /// </summary>
    /// <returns>파일이 성공적으로 업로드되었는지를 나타내는 IActionResult를 반환합니다.</returns>
    [HttpGet("uploadfiles")]
    public async Task<IActionResult> UploadFiles()
    {
        var localPath = Path.Combine(environment.WebRootPath, "files");
        await UploadFilesToBlobAsync(localPath);
        return Ok("Files uploaded successfully.");
    }

    /// <summary>
    /// Azure Blob Storage에서 파일을 다운로드하여 로컬 "files" 폴더에 저장합니다.
    /// </summary>
    /// <returns>파일이 성공적으로 다운로드되었는지를 나타내는 IActionResult를 반환합니다.</returns>
    [HttpGet("downloadfiles")]
    public async Task<IActionResult> DownloadFiles()
    {
        var localPath = Path.Combine(environment.WebRootPath, "files");
        await DownloadFilesFromBlobAsync(localPath);
        return Ok("Files downloaded successfully.");
    }

    /// <summary>
    /// 지정된 로컬 경로에서 모든 파일을 읽어 Azure Blob Storage 컨테이너에 업로드합니다.
    /// </summary>
    /// <param name="localPath">로컬 파일이 저장된 경로입니다.</param>
    /// <returns>업로드 작업을 비동기적으로 수행하는 Task입니다.</returns>
    private async Task UploadFilesToBlobAsync(string localPath)
    {
        var connectionString = $"DefaultEndpointsProtocol=https;AccountName=" +
            $"{configuration["AppKeys:AzureStorageAccount"]};AccountKey=" +
            $"{configuration["AppKeys:AzureStorageAccessKey"]};" +
            $"EndpointSuffix=core.windows.net";
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        foreach (var filePath in Directory.GetFiles(localPath, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(localPath, filePath);
            var blobClient = containerClient.GetBlobClient(relativePath);

            if (await blobClient.ExistsAsync()) // 동일한 파일이 존재하면 건너뜀
                continue;

            using var fileStream = System.IO.File.OpenRead(filePath);
            await blobClient.UploadAsync(fileStream, overwrite: false);
        }
    }

    /// <summary>
    /// Azure Blob Storage에서 파일을 다운로드하여 지정된 로컬 경로에 저장합니다.
    /// </summary>
    /// <param name="localPath">다운로드된 파일을 저장할 로컬 경로입니다.</param>
    /// <returns>다운로드 작업을 비동기적으로 수행하는 Task입니다.</returns>
    private async Task DownloadFilesFromBlobAsync(string localPath)
    {
        var connectionString = $"DefaultEndpointsProtocol=https;AccountName=" +
            $"{configuration["AppKeys:AzureStorageAccount"]};AccountKey=" +
            $"{configuration["AppKeys:AzureStorageAccessKey"]};" +
            $"EndpointSuffix=core.windows.net";
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            var downloadPath = Path.Combine(localPath, blobItem.Name);

            // 해당 디렉터리 생성
            var directoryName = Path.GetDirectoryName(downloadPath);
            if (directoryName == null)
            {
                throw new InvalidOperationException("The directory name cannot be null.");
            }
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            if (System.IO.File.Exists(downloadPath)) // 로컬에 이미 동일한 파일이 존재하면 건너뜀
                continue;

            // 파일 다운로드
            var response = await blobClient.DownloadAsync();
            using (var fileStream = System.IO.File.Create(downloadPath))
            {
                await response.Value.Content.CopyToAsync(fileStream);
            }
        }
    }
}
