using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Shared.Models.DTOs;

namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public DoctorResponseDto? Doctor { get; set; }
        public PatientResponseDto? Patient { get; set; }
        public List<AppointmentProcedureResponseDto> Procedures { get; set; } = [];
        public decimal TotalCost { get; set; }
        public decimal Discount { get; set; }
    }
}