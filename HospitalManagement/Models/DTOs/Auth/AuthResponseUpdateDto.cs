using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Auth
{
    public record AuthResponseUpdateDto
    (
        int Id,

        string Username,

        string Email,

        UserRole Role,

        bool IsActive,

        DateTime CreatedAt
    );
}
