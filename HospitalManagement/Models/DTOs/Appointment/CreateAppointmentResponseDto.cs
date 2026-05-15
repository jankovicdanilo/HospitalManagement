namespace HospitalManagement.Models.DTOs.Appointment
{
    public record CreateAppointmentResponseDto
    (
        int Id,

        int PatientId,

        int DoctorId,

        DateTime DateTime,

        TimeSpan Duration,

        string Status,

        string? Notes
    );
}
