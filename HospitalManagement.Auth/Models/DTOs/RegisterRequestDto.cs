using HospitalManagement.Auth.Models.Enums;

namespace HospitalManagement.Auth.Models.DTOs
{
    public class RegisterRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
