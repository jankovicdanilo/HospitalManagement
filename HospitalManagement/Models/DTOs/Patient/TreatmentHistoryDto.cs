namespace HospitalManagement.Models.DTOs.Patient
{
    public class TreatmentHistoryDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Medication { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
