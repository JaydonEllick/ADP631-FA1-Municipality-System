using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipality_Management_System.Models
{
    public class ReportsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportID { get; set; }

        [ForeignKey("CitizenID")]
        public int CitizenID { get; set; }

        [Required]
        public required string ReportType { get; set; }

        [Required]
        public required string Details { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Under Review";
    }
}
