using Microsoft.AspNetCore.Http;

namespace ContosoDashboard.Services
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file to storage and returns the relative path
        /// </summary>
        Task<string> UploadAsync(IFormFile file, string filePath);

        /// <summary>
        /// Downloads a file from storage as a stream
        /// </summary>
        Task<Stream> DownloadAsync(string filePath);

        /// <summary>
        /// Deletes a file from storage
        /// </summary>
        Task DeleteAsync(string filePath);

        /// <summary>
        /// Checks if a file exists in storage
        /// </summary>
        Task<bool> ExistsAsync(string filePath);
    }
}