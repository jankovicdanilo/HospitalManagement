using HospitalManagement.Appointments.Models.DTOs.Invoice;
using HospitalManagement.Appointments.Services.Interfaces;

namespace HospitalManagement.Appointments.Services.Implementations
{
    public class PdfGenerator : IPdfGenerator
    {
        public byte[] Generate(InvoiceData data)
        {
            throw new NotImplementedException();
        }
    }
}
