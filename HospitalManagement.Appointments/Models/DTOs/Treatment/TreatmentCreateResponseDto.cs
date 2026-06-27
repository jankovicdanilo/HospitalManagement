namespace HospitalManagement.Appointments.Models.DTOs.Treatment
{
    public class TreatmentCreateResponseDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Description { get; set; }
        public string? Medication { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}