using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Procedure;
using HospitalManagement.Models.Enums;

namespace HospitalManagement.Models.DTOs.Appointment
{
    public class AppointmentListResponseDto
    {
        public int Id { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public List<ProcedureResponseDto> Procedures { get; set; } = [];
        public decimal TotalCost { get; set; }
    }
}
