using Microsoft.AspNetCore.Http;

namespace QualityControlApp.Services
{
    public interface IFileService
    {
        Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadFileAsync(
            IFormFile file, 
            string folderPath, 
            string[] allowedExtensions, 
            long maxSizeInBytes = 10 * 1024 * 1024); // 10MB default

        Task<bool> DeleteFileAsync(string filePath);
        Task<bool> FileExistsAsync(string filePath);
        Task<long> GetFileSizeAsync(string filePath);
        string GetFileExtension(string fileName);
        bool IsValidFileExtension(string fileName, string[] allowedExtensions);
        bool IsValidFileSize(long fileSize, long maxSizeInBytes);
        string SanitizeFileName(string fileName);
        
        // Helper method for AOC documents specifically
        Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadAocDocumentAsync(IFormFile file);
    }
}
