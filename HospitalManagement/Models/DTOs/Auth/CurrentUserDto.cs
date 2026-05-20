using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Auth
{
    public class CurrentUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
