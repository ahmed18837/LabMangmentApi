namespace LabMangmentApi.Models.Dtos.Maintenance
{
    public class UpdateMaintenanceFullDto
    {
        public string DeviceName { get; set; }
        public string Type { get; set; }
        public string ScheduledDate { get; set; }
        public string Responsible { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string Reason { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int? Cost { get; set; }
    }
}
