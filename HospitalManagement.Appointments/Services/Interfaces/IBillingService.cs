using HospitalManagement.Appointments.Models.DTOs.Invoice;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Appointments.Services.Interfaces
{
    public interface IBillingService
    {
        Task<Result<InvoiceData>> GetInvoiceDataAsync(int appointmentId);
        Task<Result<byte[]>> GenerateInvoiceAsync(int appointmentId);
    }
}
