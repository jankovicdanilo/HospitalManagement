using HospitalManagement.Models.Enums;

namespace HospitalManagement.Models.DTOs.Appointment
{
    public class AppointmentStatusUpdateDto
    {
        public int Id { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}
