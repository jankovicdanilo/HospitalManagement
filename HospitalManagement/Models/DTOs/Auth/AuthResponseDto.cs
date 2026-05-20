using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Token { get; set; }
    }
}
