using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Shared.Models.DTOs;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentUpdateResponseDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public DoctorResponseDto? Doctor { get; set; }
        public PatientResponseDto? Patient { get; set; }
        public List<AppointmentProcedureResponseDto> Procedures { get; set; } = []; 
    }
}