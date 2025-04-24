namespace LabMangmentApi.Models.Entities
{
    public class Maintenance
    {
        public int Id { get; set; }
        public string? Priority { get; set; } //  عاليه ومتوسطه وضعيفه
        public string Status { get; set; } //  مكتمله و قيد التنفيذ ومجدوله وقيد الانتظار 
        public string Type { get; set; } //   دوريه و معايره واصلاح
        public DateTime SchedulingAt { get; set; } //
        public DateTime? StartAt { get; set; }   //
        public DateTime? EndAt { get; set; }  //
        public int Cost { get; set; } //
        public int DeviceId { get; set; } //
        public Device Device { get; set; } //
        public string? Notes { get; set; } //
        public string Reason { get; set; }  //
        public int UserId { get; set; } //
        public User user { get; set; } //

    }
}