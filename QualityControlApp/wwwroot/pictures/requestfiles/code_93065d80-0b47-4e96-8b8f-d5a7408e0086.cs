using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class AirPortRequest : BaseEntity
    {
        [Required(ErrorMessage = "The Department field is required.")]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required(ErrorMessage = "The Entry Time field is required.")]
        [Display(Name = "Entry Time")]
        public DateTime EntryTime { get; set; }

        [Required(ErrorMessage = "The Request Time field is required.")]
        [Display(Name = "Request Time")]
        public DateTime RequestTime { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Sender Name field is required.")]
        [Display(Name = "Sender Name")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "The Company Name field is required.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "The Flight Date field is required.")]
        [Display(Name = "Flight Date")]
        public DateTime FlightDate { get; set; }

        [Required(ErrorMessage = "The Aircraft Type field is required.")]
        [Display(Name = "Aircraft Type")]
        public string AircraftType { get; set; }

        [Required(ErrorMessage = "The Aircraft Registration field is required.")]
        [Display(Name = "Aircraft Registration")]
        public string AircraftRegistration { get; set; }

        [Required(ErrorMessage = "The Call Sign field is required.")]
        [Display(Name = "Call Sign")]
        public string CallSign { get; set; }

        [Required(ErrorMessage = "The Flight Path field is required.")]
        [Display(Name = "Flight Path")]
        public string FlightPath { get; set; }

        [Required(ErrorMessage = "The Landing and Takeoff Time field is required.")]
        [Display(Name = "Landing and Takeoff Time")]
        public string LandingTakeoffTime { get; set; } // Consider if this should be DateTime or separate fields

        [Required(ErrorMessage = "The Flight Purpose field is required.")]
        [Display(Name = "Flight Purpose")]
        public string FlightPurpose { get; set; }

        [Required(ErrorMessage = "The Entry and Exit Points field is required.")]
        [Display(Name = "Entry and Exit Points")]
        public string EntryExitPoints { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; } // Made nullable as it's not required

        [Required(ErrorMessage = "The Request Status field is required.")]
        [Display(Name = "Request Status")]
        public string RequestStatus { get; set; }

        // Relationship with Users table
        [Display(Name = "User who approved the request")]
        [ForeignKey("ApproverUserId")]
        public ApplicationUser? ApplicationUser { get; set; } // Made ApplicationUser nullable

        [ForeignKey("ApplicationUser")]
        public string? ApproverUserId { get; set; } // ApproverUserId is already nullable

        // Relationship with Attachments table (One-to-Many)
        [ValidateNever]
        public virtual ICollection<AirPortRequestFiles>? RequestFiles { get; set; } // Made collection nullable

        [Display(Name = "Pilot Name")] // Added Display attribute
        public string? PilotName { get; set; }

        [Display(Name = "Flight Number")] // Added Display attribute
        public string? FlightNumber { get; set; }

        [Display(Name = "Entry Point")] // Added Display attribute
        public string? EntryPoint { get; set; }

        [Display(Name = "Exit Point")] // Added Display attribute
        public string? ExitPoint { get; set; }

        [Display(Name = "Estimated Entry Time")] // Added Display attribute
        public DateTime? EstimatedEntryTime { get; set; }

        [Display(Name = "Estimated Exit Time")] // Added Display attribute
        public DateTime? EstimatedExitTime { get; set; }

        [Display(Name = "Cargo Details")] // Added Display attribute
        public string? CargoDetails { get; set; }

        [Display(Name = "Crew Count")] // Added Display attribute
        [Range(0, int.MaxValue, ErrorMessage = "Crew count cannot be negative.")] // Optional: Add Range validation
        public int? CrewCount { get; set; }

        [Display(Name = "Crew Nationalities")] // Added Display attribute
        public string? CrewNationalities { get; set; }
    }
}