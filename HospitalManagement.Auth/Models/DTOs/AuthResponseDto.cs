using HospitalManagement.Auth.Models.Enums;

namespace HospitalManagement.Auth.Models.DTOs
{
    public class AuthResponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Token { get; set; }
    }
}
