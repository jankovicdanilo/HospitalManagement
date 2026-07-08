using HospitalManagement.InvoiceService.Models.DTOs.Invoice;

namespace HospitalManagement.InvoiceService.Clients.Interfaces
{
    public interface IAppointmentServiceClient
    {
        Task<AppointmentInvoiceDto?> GetAppointmentAsync(int appointmentId);
    }
}
