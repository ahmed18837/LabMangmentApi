namespace LabMangmentApi.Models.Dtos.Maintenance
{
    public class AddScheduledMaintenanceDto
    {
        public string DeviceName { get; set; }
        public string Type { get; set; }
        public string Responsible { get; set; }
        public string Reason { get; set; }
    }
}
