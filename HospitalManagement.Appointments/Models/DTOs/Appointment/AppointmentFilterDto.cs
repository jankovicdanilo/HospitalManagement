using HospitalManagement.Appointments.Models.Enums;

namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentFilterDto
    {
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public DateOnly? Date { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public AppointmentStatus? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}