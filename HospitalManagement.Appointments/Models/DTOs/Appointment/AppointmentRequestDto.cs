using HospitalManagement.Appointments.Models.Enums;

namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentRequestDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}