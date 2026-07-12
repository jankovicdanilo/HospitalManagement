namespace HospitalManagement.InvoiceService.Models.DTOs.Invoice
{
    public class InvoiceData
    {
        public string? InvoiceNumber { get; set; }
        public DateTime IssuedDate { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Notes { get; set; }
        public List<InvoiceLineItem> Procedures { get; set; } = [];
        public string? ServiceDescription { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
