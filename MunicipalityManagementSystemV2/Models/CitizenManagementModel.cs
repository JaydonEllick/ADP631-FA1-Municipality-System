using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipality_Management_System.Models
{
    public class CitizenManagementModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //identity autoincrements.
        public int CitizenID { get; set; }

        [Required] //Add error message for this as well later?
        [StringLength(100, ErrorMessage = ("Max character limit for name is 100."))]
        public required string Fullname { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = ("Max character limit for address is 200."))]
        public required string Address { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be  10 digits.")]
        public required string PhoneNumber { get; set; }

        //Not required
        [EmailAddress]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; } //Not sure why this is DateTime. Not Like most people know what time they are born.

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow; //utc to be consistent across time zones.
        
    }
}
