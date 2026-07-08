using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.InvoiceService.Services.Interfaces;
using QuestPDF.Fluent;

namespace HospitalManagement.InvoiceService.Services.Pdf
{
    public class PdfGenerator : IPdfGenerator
    {
        public byte[] Generate(InvoiceData data)
        {
            return Document.Create(container =>
            {
                new InvoiceDocument(data).Compose(container);
            }).GeneratePdf();
        }
    }
}
