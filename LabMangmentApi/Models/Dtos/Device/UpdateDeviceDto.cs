namespace LabMangmentApi.Models.Dtos.Device
{
	public class UpdateDeviceDto
	{
		public string Name { get; set; }
		public int SerialNumber { get; set; }
		public string Category { get; set; }
		public string Location { get; set; }
		public string Status { get; set; }
		public DateTime PurchaseDate { get; set; }
		public int Lifespan { get; set; }
		public string? Notes { get; set; }
	}
}
