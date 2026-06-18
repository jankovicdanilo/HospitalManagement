namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentCreateRequestDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Notes { get; set; }
    }
}