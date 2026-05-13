namespace HospitalManagement.Models.DTOs.Appointment
{
    public record AppointmentResponseDto
    (
        int Id,

        int PatientId,

        int DoctorId,

        DateTime DateTime,

        string Status,

        string? Notes
    );
}
