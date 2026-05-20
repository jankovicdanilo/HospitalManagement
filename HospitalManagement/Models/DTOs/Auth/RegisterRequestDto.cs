using HospitalManagement.Models.Enums;

namespace HospitalManagement.Models.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
