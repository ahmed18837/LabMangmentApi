namespace LabMangmentApi.Models.Entities
{
    public class Alert
    {
        public int Id { get; set; }   //
        public string Type { get; set; } //
        public string Level { get; set; } //
        public string Title { get; set; } //
        public string Message { get; set; } //
        public DateTime CreatedAt { get; set; }  //
        public DateTime ExpiresAt { get; set; } //
        public bool IsActive { get; set; } //
        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public ICollection<AlertRecipient> AlertRecipients { get; set; }
    }
}
