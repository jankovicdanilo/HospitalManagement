using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.InvoiceService.Services.Interfaces
{
    public interface IBillingService
    {
        Task<Result<InvoiceResult>> GenerateInvoiceAsync(int appointmentId);
    }
}
