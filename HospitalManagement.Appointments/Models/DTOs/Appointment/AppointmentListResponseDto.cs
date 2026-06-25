using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Models.Enums;

namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentListResponseDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public List<AppointmentProcedureResponseDto> Procedures { get; set; } = [];
        public decimal TotalCost { get; set; }
        public decimal Discount { get; set; }
    }
}