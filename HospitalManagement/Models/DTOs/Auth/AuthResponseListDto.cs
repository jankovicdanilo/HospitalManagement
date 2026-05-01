using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Auth
{
    public record AuthResponseListDto
    (
        int id,
        string Username,
        string Email,
        UserRole Role,
        DateTime CreatedAt,
        bool IsActive
    );
}
