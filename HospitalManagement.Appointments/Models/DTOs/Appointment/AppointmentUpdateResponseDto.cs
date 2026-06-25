using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Shared.Models.DTOs.External;

namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentUpdateResponseDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public ExternalDoctorDto? Doctor { get; set; }
        public ExternalPatientDto? Patient { get; set; }
        public List<AppointmentProcedureResponseDto> Procedures { get; set; } = []; 
    }
}