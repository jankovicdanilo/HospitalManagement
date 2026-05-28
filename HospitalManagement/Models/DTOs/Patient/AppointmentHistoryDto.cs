using HospitalManagement.Models.Enums;

namespace HospitalManagement.Models.DTOs.Patient
{
    public class AppointmentHistoryDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public string DoctorName { get; set; }
        public TreatmentHistoryDto? Treatment {  get; set; } 
    }
}
