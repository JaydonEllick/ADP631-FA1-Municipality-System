using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipality_Management_System.Models
{
    public class StaffManagementModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StaffID { get; set; }

        [Required]
        [StringLength(100)]
        public required string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public required string Position { get; set; }

        [Required]
        [StringLength(100)]
        public required string Department { get; set; }

        //Unlike citizen, email is required for staff.
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required DateTime HiredDate { get; set; }
    }
}
