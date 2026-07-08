using HospitalManagement.InvoiceService.Models.Enums;

namespace HospitalManagement.InvoiceService.Services.Interfaces
{
    public interface IInvoiceDocumentGeneratorFactory
    {
        IInvoiceDocumentGenerator GetGenerator(InvoiceFormat format);
    }
}
