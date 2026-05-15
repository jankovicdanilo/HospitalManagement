using HospitalManagement.Models.Domain;

namespace HospitalManagement.Models.DTOs.Appointment
{
    public class AppointmentListResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
    }
}
