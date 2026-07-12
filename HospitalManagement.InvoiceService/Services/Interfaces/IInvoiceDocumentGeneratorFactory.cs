using HospitalManagement.InvoiceService.Models.Enums;

namespace HospitalManagement.InvoiceService.Services.Interfaces
{
    public interface IInvoiceDocumentGeneratorFactory
    {
        IInvoiceDocumentGenerator CreateGenerator(InvoiceFormat format);
    }
}
