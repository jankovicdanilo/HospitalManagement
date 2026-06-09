using HospitalManagement.Auth.Models.Enums;

namespace HospitalManagement.Auth.Models.DTOs
{
    public class UpdateUserRequestDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}
