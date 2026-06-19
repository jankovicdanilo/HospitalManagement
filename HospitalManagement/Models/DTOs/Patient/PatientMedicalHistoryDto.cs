namespace HospitalManagement.Models.DTOs.Patient
{
    public class PatientMedicalHistoryDto
    {
        public int Id { get; set; }
        public string? PatientName { get; set; }
        // TODO: cross-service — appointment history requires HTTP call to appointment microservice
        // public List<AppointmentHistoryDto> Appointments { get; set; } = new List<AppointmentHistoryDto>();
    }
}
