using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Auth
{
    public record AuthResponseDto
    (
        string Username,
        string Email,
        UserRole Role,
        string Token
    );
}
