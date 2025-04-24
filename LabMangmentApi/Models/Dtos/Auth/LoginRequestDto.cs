using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LabMangmentApi.Models.Dtos.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        [DefaultValue("user@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(64, ErrorMessage = "Password cannot exceed 64 characters.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DefaultValue("12345678")]
        public string Password { get; set; }
    }
}