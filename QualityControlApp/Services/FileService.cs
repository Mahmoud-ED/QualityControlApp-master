using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace QualityControlApp.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileService> _logger;

        // Allowed file extensions for AOC documents
        private static readonly string[] AllowedAocExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        
        // Maximum file size: 10MB
        private const long MaxFileSize = 10 * 1024 * 1024;

        public FileService(IWebHostEnvironment webHostEnvironment, ILogger<FileService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadFileAsync(
            IFormFile file, 
            string folderPath, 
            string[] allowedExtensions, 
            long maxSizeInBytes = MaxFileSize)
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    return (false, null, "No file provided.");
                }

                // Check file size
                if (!IsValidFileSize(file.Length, maxSizeInBytes))
                {
                    return (false, null, $"File size exceeds the maximum allowed size of {maxSizeInBytes / (1024 * 1024)}MB.");
                }

                // Check file extension
                if (!IsValidFileExtension(file.FileName, allowedExtensions))
                {
                    return (false, null, $"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
                }

                // Sanitize file name
                var sanitizedFileName = SanitizeFileName(file.FileName);
                var fileName = Path.GetFileNameWithoutExtension(sanitizedFileName);
                var extension = Path.GetExtension(sanitizedFileName);

                // Generate unique file name
                var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                // Create directory if it doesn't exist
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fullPath = Path.Combine(uploadsFolder, uniqueFileName);
                var relativePath = $"/{folderPath.Replace("\\", "/")}/{uniqueFileName}";

                // Save file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File uploaded successfully: {FilePath}", relativePath);
                return (true, relativePath, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
                return (false, null, "An error occurred while uploading the file.");
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
                return File.Exists(fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<long> GetFileSizeAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return 0;

                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
                
                if (File.Exists(fullPath))
                {
                    var fileInfo = new FileInfo(fullPath);
                    return fileInfo.Length;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file size: {FilePath}", filePath);
                return 0;
            }
        }

        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName)?.ToLowerInvariant() ?? string.Empty;
        }

        public bool IsValidFileExtension(string fileName, string[] allowedExtensions)
        {
            var extension = GetFileExtension(fileName);
            return allowedExtensions.Contains(extension);
        }

        public bool IsValidFileSize(long fileSize, long maxSizeInBytes)
        {
            return fileSize > 0 && fileSize <= maxSizeInBytes;
        }

        public string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            // Remove or replace invalid characters
            var sanitized = Regex.Replace(fileName, @"[^\w\s.-]", "", RegexOptions.Compiled);
            
            // Replace multiple spaces with single space
            sanitized = Regex.Replace(sanitized, @"\s+", " ", RegexOptions.Compiled);
            
            // Trim and limit length
            sanitized = sanitized.Trim();
            if (sanitized.Length > 100)
            {
                var extension = Path.GetExtension(sanitized);
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(sanitized);
                sanitized = nameWithoutExtension.Substring(0, 100 - extension.Length) + extension;
            }

            return sanitized;
        }

        // Helper method for AOC documents specifically
        public async Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadAocDocumentAsync(IFormFile file)
        {
            return await UploadFileAsync(file, "documents/landing/aoc", AllowedAocExtensions, MaxFileSize);
        }
    }
}
