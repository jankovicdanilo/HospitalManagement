namespace HospitalManagement.Models.DTOs.Patient
{
    public record CreatePatientRequestDto
    (
        string Name,

        string LastName,

        DateOnly DateOfBirth,

        string Email,

        string? Phone

    );
}
