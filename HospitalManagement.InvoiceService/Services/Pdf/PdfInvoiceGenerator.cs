using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.InvoiceService.Services.Interfaces;
using QuestPDF.Fluent;

namespace HospitalManagement.InvoiceService.Services.Pdf
{
    public class PdfInvoiceGenerator : IInvoiceDocumentGenerator
    {
        public string ContentType => "application/pdf";
        public string FileExtension => "pdf";

        public byte[] CreateDocument(InvoiceData data, string language)
        {
            return Document.Create(container =>
            {
                new PdfInvoiceDocument(data, language).Compose(container);
            }).GeneratePdf();
        }
    }
}
