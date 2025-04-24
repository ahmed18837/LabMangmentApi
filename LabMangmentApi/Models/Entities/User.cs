using LabMangmentApi.Data;
using System.ComponentModel.DataAnnotations;

namespace LabMangmentApi.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string UserType { get; set; }

        public string NationalId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } // ✅ لازم تبعت قيمة

        public DateTime CreatedAt { get; set; }

        public ICollection<Maintenance> Maintenances { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

        public ICollection<AlertRecipient> AlertRecipients { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }

}