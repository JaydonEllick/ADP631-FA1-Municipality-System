using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipality_Management_System.Models
{
    public class ServiceRequestModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestID { get; set; }

        [ForeignKey("CitizenID")]
        public int CitizenID { get; set; }

        [Required]
        [StringLength(50)]
        public required string ServiceType { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [StringLength(30)]
        public string Status { get; set; } = "Pending";

    }
}
