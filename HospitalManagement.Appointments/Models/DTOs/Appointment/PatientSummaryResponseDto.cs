namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class PatientSummaryResponseDto
    {
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public string Summary { get; set; } = null!;
    }
}
