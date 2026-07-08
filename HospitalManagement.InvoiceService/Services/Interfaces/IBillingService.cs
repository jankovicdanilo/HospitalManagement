using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.InvoiceService.Models.Enums;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.InvoiceService.Services.Interfaces
{
    public interface IBillingService
    {
        Task<Result<InvoiceResult>> GenerateInvoiceAsync(int appointmentId, InvoiceFormat format);
    }
}
