namespace HospitalManagement.Models.DTOs.Appointment
{
    public record AppointmentUpdateRequestDto
    (
        int Id,

        int PatientId,
        
        int DoctorId,

        DateTime DateTime,

        TimeSpan Duration,

        string Status,

        string Notes
    );
}
