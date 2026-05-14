namespace HospitalManagement.Models.DTOs.Patient
{
    public record PatientListDto
    (
        int Id,

        string Name,

        DateOnly DateOfBirth,

        string Email,

        string LastName,

        string? Phone
    );
}
