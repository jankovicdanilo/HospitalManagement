namespace HospitalManagement.Models.DTOs.Appointment
{
    public record CreateAppointmentRequestDto
    (
        int PatientId,

        int DoctorId,

        DateTime DateTime,

        string Status,

        string? Notes
    );
}
