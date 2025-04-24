using System.Text.Json.Serialization;

namespace LabMangmentApi.Models.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string Status { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public object User { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}