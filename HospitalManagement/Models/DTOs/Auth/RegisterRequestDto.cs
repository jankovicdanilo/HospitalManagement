using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Auth
{
    public record RegisterRequestDto(
    string Username,
    string Email,
    string Password,
    UserRole Role
);
}
