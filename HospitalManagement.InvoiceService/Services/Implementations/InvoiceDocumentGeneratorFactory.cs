using HospitalManagement.InvoiceService.Models.Enums;
using HospitalManagement.InvoiceService.Services.Docx;
using HospitalManagement.InvoiceService.Services.Interfaces;
using HospitalManagement.InvoiceService.Services.Pdf;

namespace HospitalManagement.InvoiceService.Services.Implementations
{
    public class InvoiceDocumentGeneratorFactory : IInvoiceDocumentGeneratorFactory
    {
        private readonly IEnumerable<IInvoiceDocumentGenerator> generators;

        public InvoiceDocumentGeneratorFactory(IEnumerable<IInvoiceDocumentGenerator> generators)
        {
            this.generators = generators;
        }

        public IInvoiceDocumentGenerator GetGenerator(InvoiceFormat format)
        {
            return format switch
            {
                InvoiceFormat.Pdf => generators.OfType<PdfInvoiceGenerator>().Single(),
                InvoiceFormat.Docx => generators.OfType<DocxInvoiceGenerator>().Single(),
                _ => throw new ArgumentException($"Unsupported invoice format: {format}")
            };
        }
    }
}
