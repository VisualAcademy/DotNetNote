using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace All.Services.Interfaces
{
    public interface IStorageServiceBase
    {
        Task<bool> CreateDirectory(string folderPath);

        Task<bool> DeleteAsync(string fileName, string folderPath);

        Task<bool> DeleteDirectoryAsync(string folderPath);

        Task<byte[]> DownloadAsync(string fileName, string folderPath);

        Task<string> DownloadPathAsync(string fileName, string folderPath);

        Task<MemoryStream> DownloadToMemoryStreamAsync(string fileName, string folderPath);

        Task<MemoryStream> DownloadToMemoryStreamAsync(string fileName, string folderPath, string accountId);

        string GetFolderPath(string ownerType, long ownerId, string fileType);

        string GetFolderPath(string ownerType, string ownerId, string fileType);

        Task<IDictionary<string, string>> GetMetadataAsync(string fileName, string folderPath);

        Task<string> UploadAsync(byte[] bytes, string fileName, string folderPath, bool overwrite);

        Task<string> UploadAsync(byte[] bytes, string fileName, string folderPath, bool overwrite, string accountId);

        Task<string> UploadAsync(Stream stream, string fileName, string folderPath, bool overwrite);

        Task<string> UploadAsync(Stream stream, string fileName, string folderPath, bool overwrite, string accountId);
    }
}
