using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DotNetNoteCore.Services
{
    public interface IStorageManager
    {
        /// <summary>
        /// File(Blob) Upload
        /// </summary>
        /// <returns>New FileName</returns>
        Task<string> UploadAsync(byte[] bytes, string fileName, string folderPath, bool overwrite);
        Task<string> UploadAsync(Stream stream, string fileName, string folderPath, bool overwrite);

        /// <summary>
        /// File(Blob) Download
        /// </summary>
        /// <returns>File(Blob)</returns>
        Task<byte[]> DownloadAsync(string fileName, string folderPath);

        /// <summary>
        /// File(Blob) Delete
        /// </summary>
        /// <returns>true or false</returns>
        Task<bool> DeleteAsync(string fileName, string folderPath);

        string GetFolderPath(string ownerType, long ownerId, string fileType);

        string GetFolderPath(string ownerType, string ownerId, string fileType);

        Task<bool> CreateDirectory(string folderPath);

        Task<string> DownloadPathAsync(string fileName, string folderPath);

        Task<MemoryStream> DownloadToMemoryStreamAsync(string fileName, string folderPath);


        Task<IDictionary<string, string>> GetMetadataAsync(string fileName, string folderPath);


        Task<string> UploadAsync(byte[] bytes, string fileName, string folderPath, bool overwrite, string accountId);


        Task<string> UploadAsync(Stream stream, string fileName, string folderPath, bool overwrite, string accountId);
    }
}
