using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Auth
{
    public record CurrentUserDto
    (
        int Id,
        string Username,
        string Email,
        UserRole Role,
        DateTime CreatedAt
    );
}
