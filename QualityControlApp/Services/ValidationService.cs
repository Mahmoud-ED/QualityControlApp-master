using QualityControlApp.Models.Entities;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(ILogger<ValidationService> logger)
        {
            _logger = logger;
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateLandingAsync(Landing landing)
        {
            var errors = new List<string>();

            try
            {
                // Validate email
                if (!await ValidateEmailAsync(landing.Email))
                {
                    errors.Add("Invalid email format.");
                }

                // Validate aircraft registration
                if (!await ValidateAircraftRegistrationAsync(landing.AircraftRegistration))
                {
                    errors.Add("Invalid aircraft registration format.");
                }

                // Validate flight number if provided
                if (!string.IsNullOrEmpty(landing.FlightNumber) && !await ValidateFlightNumberAsync(landing.FlightNumber))
                {
                    errors.Add("Invalid flight number format.");
                }

                // Validate date range
                if (!await ValidateDateRangeAsync(landing.ETA, landing.ETD))
                {
                    errors.Add("Estimated departure time must be within 24 hours of estimated arrival time.");
                }

                // Validate flight date is not in the past (allow some tolerance for same day)
                if (landing.DateOfFlights.Date < DateTime.Today)
                {
                    errors.Add("Flight date cannot be in the past.");
                }

                // Validate ETA is not too far in the future (e.g., 1 year)
                if (landing.ETA > DateTime.Now.AddYears(1))
                {
                    errors.Add("Estimated arrival time cannot be more than 1 year in the future.");
                }

                // Validate ETD is not too far in the future
                if (landing.ETD > DateTime.Now.AddYears(1))
                {
                    errors.Add("Estimated departure time cannot be more than 1 year in the future.");
                }

                // Validate captain name format if provided
                if (!string.IsNullOrEmpty(landing.CaptainName))
                {
                    if (!Regex.IsMatch(landing.CaptainName, @"^[a-zA-Z\s'-]+$"))
                    {
                        errors.Add("Captain name contains invalid characters.");
                    }
                }

                // Validate captain nationality format if provided
                if (!string.IsNullOrEmpty(landing.CaptainNationality))
                {
                    if (!Regex.IsMatch(landing.CaptainNationality, @"^[a-zA-Z\s]+$"))
                    {
                        errors.Add("Captain nationality contains invalid characters.");
                    }
                }

                // Validate radio call sign format if provided
                if (!string.IsNullOrEmpty(landing.RadioCallSign))
                {
                    if (!Regex.IsMatch(landing.RadioCallSign, @"^[A-Z0-9]+$"))
                    {
                        errors.Add("Radio call sign must contain only uppercase letters and numbers.");
                    }
                }

                // Validate request status
                if (!Regex.IsMatch(landing.RequestStatus, @"^(Pending|Approved|Rejected)$"))
                {
                    errors.Add("Invalid request status. Must be Pending, Approved, or Rejected.");
                }

                // Validate string lengths
                if (landing.OperatorName?.Length > 200)
                {
                    errors.Add("Operator name exceeds maximum length of 200 characters.");
                }

                if (landing.AircraftType?.Length > 100)
                {
                    errors.Add("Aircraft type exceeds maximum length of 100 characters.");
                }

                if (landing.Route?.Length > 200)
                {
                    errors.Add("Route exceeds maximum length of 200 characters.");
                }

                if (landing.AirportOfLanding?.Length > 100)
                {
                    errors.Add("Airport of landing exceeds maximum length of 100 characters.");
                }

                if (landing.NatureOfPaxOrCargo?.Length > 1000)
                {
                    errors.Add("Nature of passengers or cargo exceeds maximum length of 1000 characters.");
                }

                if (landing.CrewDetails?.Length > 2000)
                {
                    errors.Add("Crew details exceeds maximum length of 2000 characters.");
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating landing data");
                errors.Add("An error occurred during validation.");
                return (false, errors);
            }
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var emailAttribute = new EmailAddressAttribute();
                return emailAttribute.IsValid(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating email: {Email}", email);
                return false;
            }
        }

        public async Task<bool> ValidateAircraftRegistrationAsync(string registration)
        {
            if (string.IsNullOrEmpty(registration))
                return false;

            // Standard aircraft registration format: letters and numbers, typically 3-20 characters
            return Regex.IsMatch(registration, @"^[A-Z0-9-]{3,20}$");
        }

        public async Task<bool> ValidateFlightNumberAsync(string flightNumber)
        {
            if (string.IsNullOrEmpty(flightNumber))
                return true; // Optional field

            // More flexible flight number format: letters and numbers, 2-10 characters
            // Examples: AA123, BA456, EK789, QR1234, etc.
            return Regex.IsMatch(flightNumber, @"^[A-Z0-9-]{2,10}$");
        }

        public async Task<bool> ValidateDateTimeAsync(DateTime dateTime, string fieldName)
        {
            try
            {
                // Check if date is not too far in the past (more than 1 year)
                if (dateTime < DateTime.Now.AddYears(-1))
                {
                    _logger.LogWarning("Date {FieldName} is too far in the past: {DateTime}", fieldName, dateTime);
                    return false;
                }

                // Check if date is not too far in the future (more than 1 year)
                if (dateTime > DateTime.Now.AddYears(1))
                {
                    _logger.LogWarning("Date {FieldName} is too far in the future: {DateTime}", fieldName, dateTime);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating date time for {FieldName}: {DateTime}", fieldName, dateTime);
                return false;
            }
        }

        public async Task<bool> ValidateDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                // For landing requests, be very flexible with ETA/ETD times
                // Only validate that times are within reasonable bounds
                var timeDifference = endDate - startDate;
                
                // Allow any time difference between -24 hours and +24 hours
                // This covers all realistic landing scenarios
                if (timeDifference.TotalHours < -24)
                {
                    _logger.LogWarning("ETD {EndDate} is more than 24 hours before ETA {StartDate}", endDate, startDate);
                    return false;
                }

                if (timeDifference.TotalHours > 24)
                {
                    _logger.LogWarning("Time difference between ETA and ETD is too large: {Hours} hours", timeDifference.TotalHours);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating date range: {StartDate} to {EndDate}", startDate, endDate);
                return false;
            }
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateFileAsync(IFormFile file, string[] allowedExtensions, long maxSize)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return (false, "No file provided.");
                }

                // Check file size
                if (file.Length > maxSize)
                {
                    return (false, $"File size exceeds the maximum allowed size of {maxSize / (1024 * 1024)}MB.");
                }

                // Check file extension
                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return (false, $"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
                }

                // Check file name for suspicious patterns
                if (ContainsSuspiciousPatterns(file.FileName))
                {
                    return (false, "File name contains suspicious patterns.");
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating file: {FileName}", file?.FileName);
                return (false, "An error occurred during file validation.");
            }
        }

        private bool ContainsSuspiciousPatterns(string fileName)
        {
            var suspiciousPatterns = new[]
            {
                @"\.\.", // Path traversal
                @"<script", // Script injection
                @"javascript:", // JavaScript protocol
                @"data:", // Data URI
                @"vbscript:", // VBScript protocol
                @"onload", // Event handlers
                @"onerror", // Event handlers
                @"onclick" // Event handlers
            };

            return suspiciousPatterns.Any(pattern => Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase));
        }
    }
}
