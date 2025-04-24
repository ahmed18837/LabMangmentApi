namespace LabMangmentApi.Models.Dtos.Maintenance
{
    public class MaintenanceResponseDto
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string MaintenanceType { get; set; }
        public string ScheduledDate { get; set; }
        public string Responsible { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string Notes { get; set; }
        public string Reason { get; set; }
        public int? Cost { get; set; }
    }
}
