using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.InvoiceService.Services.Interfaces;
using QuestPDF.Fluent;

namespace HospitalManagement.InvoiceService.Services.Pdf
{
    public class PdfInvoiceGenerator : IInvoiceDocumentGenerator
    {
        public string ContentType => "application/pdf";
        public string FileExtension => "pdf";

        public byte[] Generate(InvoiceData data)
        {
            return Document.Create(container =>
            {
                new PdfInvoiceDocument(data).Compose(container);
            }).GeneratePdf();
        }
    }
}
