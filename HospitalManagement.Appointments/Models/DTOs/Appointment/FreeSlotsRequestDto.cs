namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class FreeSlotsRequestDto
    {
        public int DoctorId { get; set; }
        public DateOnly Date { get; set; }
    }
}