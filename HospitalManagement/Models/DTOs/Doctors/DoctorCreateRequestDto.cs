namespace HospitalManagement.Models.DTOs.Doctor
{
    public record DoctorCreateRequestDto
    (
        string FirstName,

        string LastName,

        string Specialization,

        string Email,

        string? Phone
    );
}
