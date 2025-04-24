using System.ComponentModel.DataAnnotations;

namespace LabMangmentApi.Models.Entities
{
    public class AlertRecipient
    {

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int AlertId { get; set; } // لازم يتحدد
        public Alert Alert { get; set; }
    }
}