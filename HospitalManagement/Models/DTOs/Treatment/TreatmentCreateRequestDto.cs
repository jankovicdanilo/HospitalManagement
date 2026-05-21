namespace HospitalManagement.Models.DTOs.Treatment
{
    public class TreatmentCreateRequestDto
    {   
        public int AppointmentId { get; set; }
        public string Description { get; set; }
        public string? Medication { get; set; }
    }
}
