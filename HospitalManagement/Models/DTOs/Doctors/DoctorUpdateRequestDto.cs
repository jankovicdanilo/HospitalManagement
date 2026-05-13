namespace HospitalManagement.Models.DTOs.Doctor
{
    public record DoctorUpdateRequestDto
    (
        int Id,

        string FirstName,

        string LastName,

        string Specialization,

        string Email,

        string? Phone
    );
    
}
