using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.Models.Entities
{
    public class Landing : BaseEntity
    {

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        public string Period { get; set; }
        public DateTime DateOfFlights { get; set; }

        public string OperatorName { get; set; }
        public string OperatorAddress { get; set; }

        public string AircraftType { get; set; }
        public string AircraftRegistration { get; set; }

        public string RadioCallSign { get; set; }
        public string FlightNumber { get; set; }

        public string Route { get; set; }
        public string AirportOfLanding { get; set; }

        public DateTime ETA { get; set; } // Estimated Time of Arrival
        public DateTime ETD { get; set; } // Estimated Time of Departure

        public string PurposeOfFlight { get; set; }

        public string NatureOfPaxOrCargo { get; set; } // Full details of Pax/Cargo

        public string Consignor { get; set; }
        public string Consignee { get; set; }

        public string CaptainName { get; set; }
        public string CaptainNumber { get; set; }
        public string CaptainNationality { get; set; }

        public string CrewDetails { get; set; }

        public string? AocDocumentPath { get; set; } // To store file path if you upload AOC document

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

    }
}
