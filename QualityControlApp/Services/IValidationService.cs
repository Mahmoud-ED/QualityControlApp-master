using QualityControlApp.Models.Entities;

namespace QualityControlApp.Services
{
    public interface IValidationService
    {
        Task<(bool IsValid, List<string> Errors)> ValidateLandingAsync(Landing landing);
        Task<bool> ValidateEmailAsync(string email);
        Task<bool> ValidateAircraftRegistrationAsync(string registration);
        Task<bool> ValidateFlightNumberAsync(string flightNumber);
        Task<bool> ValidateDateTimeAsync(DateTime dateTime, string fieldName);
        Task<bool> ValidateDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<(bool IsValid, string ErrorMessage)> ValidateFileAsync(IFormFile file, string[] allowedExtensions, long maxSize);
    }
}
