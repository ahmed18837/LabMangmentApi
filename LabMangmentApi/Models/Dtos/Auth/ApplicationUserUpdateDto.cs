using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LabMangmentApi.Models.Dtos.Auth
{
    public class ApplicationUserUpdateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        [DefaultValue("الأسم")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [DefaultValue("user@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "NationalId is required.")]
        [DefaultValue("12345678912345")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [DefaultValue("طالب")]
        public string Type { get; set; } // student, admin, technician

        [Required(ErrorMessage = "Status is required")]
        [DefaultValue("نشط")]
        public string Status { get; set; } // active, inactive

        public DateTime RegistrationDate { get; set; }
    }

}
