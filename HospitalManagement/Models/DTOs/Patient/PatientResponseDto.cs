namespace HospitalManagement.Models.DTOs.Patient
{
    public record PatientResponseDto
    (
        int Id,

        string Name,

        string LastName,

        DateOnly DateOfBirth,

        string Email,

        string? Phone
    );
}
