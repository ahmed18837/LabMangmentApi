namespace LabMangmentApi.Models.Dtos.Device
{
    public class DeviceUsageReportDto
    {
        public string Device { get; set; }
        public string DeviceId { get; set; }
        public string User { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double Hours { get; set; }
    }
}
