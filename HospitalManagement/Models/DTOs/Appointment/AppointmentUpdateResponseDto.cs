namespace HospitalManagement.Models.DTOs.Appointment
{
    public record AppointmentUpdateResponseDto
    (
        int Id,

        int PatientId,

        int DoctorId,

        DateTime DateTime,

        string Status,

        string Notes
    );
}
