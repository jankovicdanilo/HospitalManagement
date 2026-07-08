namespace HospitalManagement.InvoiceService.Models.DTOs.Invoice
{
    public class AppointmentInvoiceDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public InvoiceDoctorDto? Doctor { get; set; }
        public InvoicePatientDto? Patient { get; set; }
        public List<InvoiceProcedureDto> Procedures { get; set; } = [];
        public decimal TotalCost { get; set; }
        public decimal Discount { get; set; }
    }
}
