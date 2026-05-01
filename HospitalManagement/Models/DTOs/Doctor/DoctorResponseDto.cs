namespace HospitalManagement.Models.DTOs.Doctor
{
    public record DoctorResponseDto
    (
        int Id,

        string FirstName,

        string LastName,

        string Specialization,

        string Email,

        string? Phone
    );
}
