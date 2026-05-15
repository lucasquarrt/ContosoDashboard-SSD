using Microsoft.AspNetCore.Http;

namespace ContosoDashboard.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storagePath;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            // Store files in AppData/uploads relative to content root
            _storagePath = Path.Combine(environment.ContentRootPath, "AppData", "uploads");

            // Ensure directory exists
            Directory.CreateDirectory(_storagePath);
        }

        public async Task<string> UploadAsync(IFormFile file, string filePath)
        {
            var fullPath = GetFullPath(filePath);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task<Stream> DownloadAsync(string filePath)
        {
            var fullPath = GetFullPath(filePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        }

        public Task DeleteAsync(string filePath)
        {
            var fullPath = GetFullPath(filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string filePath)
        {
            var fullPath = GetFullPath(filePath);
            return Task.FromResult(File.Exists(fullPath));
        }

        private string GetFullPath(string filePath)
        {
            // Ensure path is safe - prevent directory traversal
            var fullPath = Path.GetFullPath(Path.Combine(_storagePath, filePath));

            // Verify the path is within our storage directory
            if (!fullPath.StartsWith(_storagePath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid file path");
            }

            return fullPath;
        }
    }
}