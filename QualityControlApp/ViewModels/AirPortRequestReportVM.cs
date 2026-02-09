using QualityControlApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels
{
    public class AirPortRequestReportVM
    {
        [Display(Name = "Report Type")]
        public string ReportType { get; set; } = "Summary";

        [Display(Name = "Date From")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Display(Name = "Request Status")]
        public string? RequestStatus { get; set; }

        [Display(Name = "Aircraft Type")]
        public string? AircraftType { get; set; }

        [Display(Name = "Flight Purpose")]
        public string? FlightPurpose { get; set; }

        [Display(Name = "Crew Count From")]
        public int? CrewCountFrom { get; set; }

        [Display(Name = "Crew Count To")]
        public int? CrewCountTo { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Pilot Name")]
        public string? PilotName { get; set; }

        [Display(Name = "Flight Number")]
        public string? FlightNumber { get; set; }

        [Display(Name = "Entry Point")]
        public string? EntryPoint { get; set; }

        [Display(Name = "Exit Point")]
        public string? ExitPoint { get; set; }

        [Display(Name = "Export Format")]
        public string ExportFormat { get; set; } = "View";

        // Available report types
        public static readonly List<string> ReportTypes = new List<string>
        {
            "Summary",
            "ByCompany",
            "ByDate",
            "ByPassengerCount",
            "ByStatus",
            "ByAircraftType",
            "ByDepartment",
            "ByFlightPurpose",
            "Detailed"
        };

        // Available statuses
        public static readonly List<string> Statuses = new List<string>
        {
            "All",
            "Pending",
            "Approved",
            "Rejected"
        };

        // Available export formats
        public static readonly List<string> ExportFormats = new List<string>
        {
            "View",
            "PDF",
            "Excel"
        };
    }

    public class AirPortRequestReportResultVM
    {
        public List<AirPortRequest> Requests { get; set; } = new List<AirPortRequest>();
        public AirPortRequestReportVM Filters { get; set; } = new AirPortRequestReportVM();
        public ReportSummaryVM Summary { get; set; } = new ReportSummaryVM();
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    public class ReportSummaryVM
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalCrewCount { get; set; }
        public int UniqueCompanies { get; set; }
        public int UniqueAircraftTypes { get; set; }
        public Dictionary<string, int> RequestsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> RequestsByCompany { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> RequestsByAircraftType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> RequestsByDepartment { get; set; } = new Dictionary<string, int>();
        public Dictionary<DateTime, int> RequestsByDate { get; set; } = new Dictionary<DateTime, int>();
    }
}
