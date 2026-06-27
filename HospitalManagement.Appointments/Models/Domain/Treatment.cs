namespace HospitalManagement.Appointments.Models.Domain
{
    public class Treatment
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Description { get; set; }
        public string? Medication { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}