namespace HospitalManagement.QueryService.Models.DTOs.Patient
{
    public class AppointmentHistoryDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public string DoctorName { get; set; } = null!;
        public decimal TotalCost { get; set; }
        public decimal Discount { get; set; }
        public List<TreatmentHistoryDto> Treatments { get; set; } = [];
    }
}