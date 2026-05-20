namespace HospitalManagement.Models.DTOs.Appointment
{
    public class AppointmentUpdateResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
