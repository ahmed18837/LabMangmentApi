namespace LabMangmentApi.Models.Dtos.Device
{
	public class DeviceDetailsDto
	{
		public int Id { get; set; }
		public int SerialNumber { get; set; }
		public string Name { get; set; }
		public string Status { get; set; }
		public string Category { get; set; }
		public string Location { get; set; }
		public DateTime PurchaseDate { get; set; }
		public int Lifespan { get; set; }
		public DateTime? LastMaintenance { get; set; }
		public string Notes { get; set; }
		public List<MaintenanceDto> MaintenanceHistory { get; set; }
	}
	public class MaintenanceDto
	{
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public string Status { get; set; }
	}
}
