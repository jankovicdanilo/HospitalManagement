namespace HospitalManagement.Auth.Models.DTOs
{
    public class ServiceTokenCreateResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
