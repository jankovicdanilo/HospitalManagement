namespace HospitalManagement.QueryService.Models.DTOs.Patient
{
    public class TreatmentDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Description { get; set; } = null!;
        public string? Medication { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}