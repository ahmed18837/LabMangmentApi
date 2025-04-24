using System.ComponentModel.DataAnnotations;

namespace LabMangmentApi.Models.Entities
{
    public class Reservation
    {
        public int Id { get; set; } //       
        public DateOnly Date { get; set; } //
        public TimeSpan StartTime { get; set; } //
        public TimeSpan EndTime { get; set; } //

        public string? Purpose { get; set; }
        public int DeviceId { get; set; } //
        public Device Device { get; set; } //

        public int UserId { get; set; } //
        public User User { get; set; }  //

    }
}