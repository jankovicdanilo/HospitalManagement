using HospitalManagement.Appointments.Models.Enums;

namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentStatusUpdateDto
    {
        public int Id { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}