namespace HospitalManagement.QueryService.Models.DTOs.Patient
{
    public class PatientMedicalHistoryDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public List<AppointmentHistoryDto> Appointments { get; set; } = [];
    }
}