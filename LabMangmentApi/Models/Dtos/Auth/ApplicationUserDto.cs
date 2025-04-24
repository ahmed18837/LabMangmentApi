namespace LabMangmentApi.Models.Dtos.Auth
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }

}
