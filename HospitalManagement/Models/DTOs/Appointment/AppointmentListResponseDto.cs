using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Appointment
{
    public record AppointmentListResponseDto
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
