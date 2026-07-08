using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.InvoiceService.Services.Interfaces;

namespace HospitalManagement.InvoiceService.Services.Docx
{
    public class DocxInvoiceGenerator : IInvoiceDocumentGenerator
    {
        public string ContentType => "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        public string FileExtension => "docx";

        public byte[] Generate(InvoiceData data)
        {
            using var memoryStream = new MemoryStream();

            using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                new DocxInvoiceDocument(data).Compose(mainPart);
                mainPart.Document?.Save();
            }

            return memoryStream.ToArray();
        }
    }
}
