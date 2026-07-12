using HospitalManagement.InvoiceService.Models.DTOs.Invoice;

namespace HospitalManagement.InvoiceService.Services.Interfaces
{
    public interface IInvoiceDocumentGenerator
    {
        byte[] CreateDocument(InvoiceData data);
        string ContentType { get; }
        string FileExtension { get; }
    }
}
