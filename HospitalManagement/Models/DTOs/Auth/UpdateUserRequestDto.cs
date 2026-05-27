using HospitalManagement.Models.Enums;

namespace HospitalManagement.Models.DTOs.Auth
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
