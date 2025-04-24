using LabMangmentApi.Models.Entities;

public class Device
{
    public int Id { get; set; }
    public int SerialNumber { get; set; } //
    public string Name { get; set; }
    public string LocationName { get; set; } //
    public string CategoryName { get; set; } //
    public string Status { get; set; } //   Available,       Unavailable,       UnderMaintenance
    public DateTime PurchaseDate { get; set; } //
    public int Lifespan { get; set; } //
    public string? Notes { get; set; } //
    public int CurrentHour { get; set; } //
    public int MaximumHour { get; set; } // 
    public DateTime? LastMaintenanceDate { get; set; } // الصيانه عموما
    public DateTime? NextMaintenanceDate { get; set; } //معياريه بس 
    public ICollection<Reservation> Reservations { get; set; }
    public ICollection<Maintenance> MaintenanceReports { get; set; }
    public ICollection<Alert> Alerts { get; set; }

}