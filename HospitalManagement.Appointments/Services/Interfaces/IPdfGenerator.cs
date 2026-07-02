using HospitalManagement.Appointments.Models.DTOs.Invoice;

namespace HospitalManagement.Appointments.Services.Interfaces
{
    public interface IPdfGenerator
    {
        byte[] Generate(InvoiceData data);
    }
}
