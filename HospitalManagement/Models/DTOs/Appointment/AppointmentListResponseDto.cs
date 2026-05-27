using HospitalManagement.Models.Domain;
using HospitalManagement.Models.Enums;

namespace HospitalManagement.Models.DTOs.Appointment
{
    public class AppointmentListResponseDto
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
