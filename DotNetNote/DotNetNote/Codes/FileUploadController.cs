using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VisualAcademy.Codes;

/// <summary>
/// Azure Blob Storage에 파일 업로드 및 다운로드 작업을 처리합니다.
/// wwwroot/files 폴더를 기준으로 Blob 컨테이너(files)와 파일을 동기화합니다.
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
    /// 로컬 "files" 폴더의 모든 파일을 Azure Blob Storage로 업로드합니다.
    /// 동일한 Blob이 이미 존재할 경우:
    /// - Blob이 0바이트이고 로컬 파일이 0바이트가 아니면 덮어씁니다.
    /// - 그 외에는 업로드를 건너뜁니다.
    /// </summary>
    /// <returns>업로드 결과 메시지를 반환합니다.</returns>
    [HttpGet("uploadfiles")]
    public async Task<IActionResult> UploadFiles()
    {
        var localPath = Path.Combine(environment.WebRootPath, "files");
        await UploadFilesToBlobAsync(localPath);
        return Ok("Files uploaded successfully.");
    }

    /// <summary>
    /// Azure Blob Storage의 모든 파일을 로컬 "files" 폴더로 다운로드합니다.
    /// 로컬에 동일한 파일이 이미 존재하면 다운로드를 건너뜁니다.
    /// </summary>
    /// <returns>다운로드 결과 메시지를 반환합니다.</returns>
    [HttpGet("downloadfiles")]
    public async Task<IActionResult> DownloadFiles()
    {
        var localPath = Path.Combine(environment.WebRootPath, "files");
        await DownloadFilesFromBlobAsync(localPath);
        return Ok("Files downloaded successfully.");
    }

    /// <summary>
    /// 지정된 로컬 경로(localPath)의 모든 파일을 Blob Storage 컨테이너에 업로드합니다.
    /// 하위 디렉터리까지 모두 포함하며, 상대 경로를 Blob 이름으로 사용합니다.
    ///
    /// 0byte 이슈 보완:
    /// - 동일한 Blob이 존재할 때 Blob 크기가 0바이트이고 로컬 파일 크기가 0보다 크면 덮어쓰기 업로드를 수행합니다.
    /// - 그 외의 경우에는 업로드를 생략합니다.
    /// </summary>
    /// <param name="localPath">업로드할 로컬 파일 루트 경로입니다.</param>
    private async Task UploadFilesToBlobAsync(string localPath)
    {
        var connectionString =
            $"DefaultEndpointsProtocol=https;" +
            $"AccountName={configuration["AppKeys:AzureStorageAccount"]};" +
            $"AccountKey={configuration["AppKeys:AzureStorageAccessKey"]};" +
            $"EndpointSuffix=core.windows.net";

        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        // 컨테이너가 없으면 생성
        await containerClient.CreateIfNotExistsAsync();

        foreach (var filePath in Directory.GetFiles(localPath, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(localPath, filePath);
            var blobClient = containerClient.GetBlobClient(relativePath);

            var localFileInfo = new FileInfo(filePath);
            bool overwrite = false;

            // 동일한 Blob이 존재하는 경우에만 크기 비교로 0byte 복구 여부를 판단
            if (await blobClient.ExistsAsync())
            {
                var blobProperties = await blobClient.GetPropertiesAsync();
                var blobSize = blobProperties.Value.ContentLength;

                // Blob이 0바이트이고 로컬 파일이 정상 크기(>0)라면 덮어쓰기 업로드
                if (blobSize == 0 && localFileInfo.Length > 0)
                {
                    overwrite = true;
                }
                else
                {
                    continue; // 그 외에는 업로드 생략
                }
            }

            using var fileStream = System.IO.File.OpenRead(filePath);
            await blobClient.UploadAsync(fileStream, overwrite: overwrite);
        }
    }

    /// <summary>
    /// Azure Blob Storage 컨테이너의 모든 파일을 로컬 경로(localPath)로 다운로드합니다.
    /// 하위 디렉터리 구조를 유지하며, 로컬에 동일 파일이 있으면 다운로드를 생략합니다.
    /// </summary>
    /// <param name="localPath">다운로드된 파일을 저장할 로컬 루트 경로입니다.</param>
    private async Task DownloadFilesFromBlobAsync(string localPath)
    {
        var connectionString =
            $"DefaultEndpointsProtocol=https;" +
            $"AccountName={configuration["AppKeys:AzureStorageAccount"]};" +
            $"AccountKey={configuration["AppKeys:AzureStorageAccessKey"]};" +
            $"EndpointSuffix=core.windows.net";

        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            var downloadPath = Path.Combine(localPath, blobItem.Name);

            // 대상 디렉터리 생성
            var directoryName = Path.GetDirectoryName(downloadPath);
            if (directoryName == null)
            {
                throw new InvalidOperationException("The directory name cannot be null.");
            }

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            // 로컬에 이미 파일이 있으면 다운로드 생략
            if (System.IO.File.Exists(downloadPath))
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