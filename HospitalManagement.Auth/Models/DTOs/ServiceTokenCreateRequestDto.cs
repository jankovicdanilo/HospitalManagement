namespace HospitalManagement.Auth.Models.DTOs
{
    public class ServiceTokenCreateRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int ExpiresInDays { get; set; } = 30;
    }
}
