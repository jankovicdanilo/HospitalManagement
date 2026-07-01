namespace HospitalManagement.QueryService.Models.DTOs.Patient
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public List<AppointmentProcedureDto> Procedures { get; set; } = [];
        public TreatmentDto? Treatment { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Discount { get; set; }
    }
}