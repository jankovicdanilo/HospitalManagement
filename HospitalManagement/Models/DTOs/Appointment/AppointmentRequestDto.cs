using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Appointment
{
    public record AppointmentRequestDto
    (
        int Id,

        int PatientId,

        int DoctorId,

        DateTime DateTime,

        string Status,

        string? Notes
    );
}
