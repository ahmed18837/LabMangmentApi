using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabMangmentApi.Models.Dtos.Reservation
{
    public class DateRequestDTO
    {
        [Column(TypeName = "DATE")]
        [DefaultValue("2024-12-09")]
        public DateOnly Date { get; set; }

    }
}
