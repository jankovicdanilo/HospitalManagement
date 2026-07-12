namespace HospitalManagement.InvoiceService.Models.DTOs.Invoice
{
    public class InvoiceProcedureDto
    {
        public string ProcedureName { get; set; } = null!;
        public decimal ProcedurePrice { get; set; }
    }
}
