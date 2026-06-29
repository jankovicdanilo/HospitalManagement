using HospitalManagement.Appointments.Models.DTOs.Invoice;
using HospitalManagement.Appointments.Services.Interfaces;
using QuestPDF.Fluent;

namespace HospitalManagement.Appointments.Services.Pdf
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
