namespace HospitalManagement.Models.DTOs.Appointment
{
    public record CreateAppointmentRequestDto
    (
        int PatientId,

        int DoctorId,

        DateTime DateTime,

        TimeSpan Duration,

        string Status,

        string? Notes
    );
}
