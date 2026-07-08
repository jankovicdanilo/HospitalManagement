using HospitalManagement.InvoiceService.Models.DTOs.Invoice;

namespace HospitalManagement.InvoiceService.Services.Interfaces
{
    public interface IPdfGenerator
    {
        byte[] Generate(InvoiceData data);
    }
}
