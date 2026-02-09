using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class Landing : BaseEntity
    {
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(50, ErrorMessage = "Period cannot exceed 50 characters.")]
        [Display(Name = "Period")]
        public string? Period { get; set; }

        [Required(ErrorMessage = "Flight date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Flight Date")]
        public DateTime DateOfFlights { get; set; }

        [Required(ErrorMessage = "Operator name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Operator name must be between 2 and 200 characters.")]
        [Display(Name = "Operator Name")]
        public string OperatorName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Operator address cannot exceed 500 characters.")]
        [Display(Name = "Operator Address")]
        public string? OperatorAddress { get; set; }

        [Required(ErrorMessage = "Aircraft type is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Aircraft type must be between 2 and 100 characters.")]
        [Display(Name = "Aircraft Type")]
        public string AircraftType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Aircraft registration is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Aircraft registration must be between 3 and 20 characters.")]
        [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Aircraft registration must contain only uppercase letters, numbers, and hyphens.")]
        [Display(Name = "Aircraft Registration")]
        public string AircraftRegistration { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Radio call sign cannot exceed 20 characters.")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Radio call sign must contain only uppercase letters and numbers.")]
        [Display(Name = "Radio Call Sign")]
        public string? RadioCallSign { get; set; }

        [StringLength(20, ErrorMessage = "Flight number cannot exceed 20 characters.")]
        [RegularExpression(@"^[A-Z0-9-]{2,20}$", ErrorMessage = "Flight number must be 2-20 characters and contain only uppercase letters, numbers, and hyphens.")]
        [Display(Name = "Flight Number")]
        public string? FlightNumber { get; set; }

        [Required(ErrorMessage = "Route is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Route must be between 3 and 200 characters.")]
        [Display(Name = "Route")]
        public string Route { get; set; } = string.Empty;

        [Required(ErrorMessage = "Airport of landing is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Airport of landing must be between 3 and 100 characters.")]
        [Display(Name = "Airport of Landing")]
        public string AirportOfLanding { get; set; } = string.Empty;

        [Required(ErrorMessage = "Estimated time of arrival is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Estimated Time of Arrival")]
        public DateTime ETA { get; set; }

        [Required(ErrorMessage = "Estimated time of departure is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Estimated Time of Departure")]
        public DateTime ETD { get; set; }

        [StringLength(100, ErrorMessage = "Purpose of flight cannot exceed 100 characters.")]
        [Display(Name = "Purpose of Flight")]
        public string? PurposeOfFlight { get; set; }

        [StringLength(1000, ErrorMessage = "Nature of passengers or cargo cannot exceed 1000 characters.")]
        [Display(Name = "Nature of Passengers or Cargo")]
        public string? NatureOfPaxOrCargo { get; set; }

        [StringLength(200, ErrorMessage = "Consignor cannot exceed 200 characters.")]
        [Display(Name = "Consignor")]
        public string? Consignor { get; set; }

        [StringLength(200, ErrorMessage = "Consignee cannot exceed 200 characters.")]
        [Display(Name = "Consignee")]
        public string? Consignee { get; set; }

        [StringLength(100, ErrorMessage = "Captain name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Captain name can only contain letters, spaces, hyphens, and apostrophes.")]
        [Display(Name = "Captain Name")]
        public string? CaptainName { get; set; }

        [StringLength(50, ErrorMessage = "Captain number cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Captain number must contain only uppercase letters, numbers, and hyphens.")]
        [Display(Name = "Captain Number")]
        public string? CaptainNumber { get; set; }

        [StringLength(50, ErrorMessage = "Captain nationality cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Captain nationality can only contain letters and spaces.")]
        [Display(Name = "Captain Nationality")]
        public string? CaptainNationality { get; set; }

        [StringLength(2000, ErrorMessage = "Crew details cannot exceed 2000 characters.")]
        [Display(Name = "Crew Details")]
        public string? CrewDetails { get; set; }

        [StringLength(500, ErrorMessage = "AOC document path cannot exceed 500 characters.")]
        [Display(Name = "AOC Document Path")]
        public string? AocDocumentPath { get; set; }

        [Required(ErrorMessage = "Request status is required.")]
        [StringLength(20, ErrorMessage = "Request status cannot exceed 20 characters.")]
        [RegularExpression(@"^(Pending|Approved|Rejected)$", ErrorMessage = "Request status must be Pending, Approved, or Rejected.")]
        [Display(Name = "Request Status")]
        public string RequestStatus { get; set; } = "Pending";

        // Relationship with Users table
        [Display(Name = "User who approved the request")]
        [ForeignKey("ApproverUserId")]
        public ApplicationUser? ApplicationUser { get; set; } // Made ApplicationUser nullable

        [ForeignKey("ApplicationUser")]
        public string? ApproverUserId { get; set; } // ApproverUserId is already nullable

        // Relationship with Attachments table (One-to-Many)
        [ValidateNever]
        public virtual ICollection<AirPortRequestFiles>? RequestFiles { get; set; } // Made collection nullable

    }
}
