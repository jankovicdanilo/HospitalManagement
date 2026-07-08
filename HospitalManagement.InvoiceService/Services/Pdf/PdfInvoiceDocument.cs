using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HospitalManagement.InvoiceService.Services.Pdf
{
    public class PdfInvoiceDocument : IDocument
    {
        private readonly InvoiceData data;

        public PdfInvoiceDocument(InvoiceData data)
        {
            this.data = data;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        public void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("City Hospital")
                        .Bold().FontSize(20);
                    col.Item().Text("Medical Invoice")
                        .FontSize(12).FontColor(Colors.Grey.Medium); 
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignRight().Text($"Invoice: {data.InvoiceNumber}")
                        .Bold().FontSize(12);
                    col.Item().AlignRight().Text($"Date: {data.IssuedDate:dd/MM/yyyy}")
                        .FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                col.Spacing(20);

                col.Item().Element(ComposePatientInfo);
                col.Item().Element(ComposeProceduresTable);
                col.Item().Element(ComposeTotals);

                if (!string.IsNullOrEmpty(data.Notes))
                {
                    col.Item().Element(ComposeNotes);
                }
            });
        }

        private void ComposePatientInfo(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Patient").Bold().FontColor(Colors.Grey.Medium);
                    col.Item().Text(data.PatientName).Bold();
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Doctor").Bold().FontColor(Colors.Grey.Medium);
                    col.Item().Text(data.DoctorName).Bold();
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Appointment Date").Bold().FontColor(Colors.Grey.Medium);
                    col.Item().Text(data.AppointmentDate.ToString("dd/MM/yyyy HH:mm"));
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Duration").Bold().FontColor(Colors.Grey.Medium);
                    col.Item().Text($"{data.Duration.TotalMinutes} min");
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Status").Bold().FontColor(Colors.Grey.Medium);
                    col.Item().Text(data.Status.ToString()).Bold();
                });
            });
        }

        private void ComposeProceduresTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                        .Text("Procedure").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                        .AlignRight().Text("Price").Bold();
                });

                foreach(var item in data.Procedures)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .Padding(5).Text(item.Name);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .Padding(5).AlignRight().Text($"{item.Price:C}");
                }
            });
        }

        private void ComposeTotals(IContainer container)
        {
            container.AlignRight().Column(col =>
            {
                col.Spacing(5);

                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("Subtotal").FontColor(Colors.Grey.Medium);
                    row.ConstantItem(100).AlignRight().Text($"{data.Subtotal:C}");
                });

                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("Discount").FontColor(Colors.Grey.Medium);
                    row.ConstantItem(100).AlignRight().Text($"-{data.Discount:C}")
                        .FontColor(Colors.Red.Medium);
                });

                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("Total").Bold().FontSize(12);
                    row.ConstantItem(100).AlignRight().Text($"{data.TotalAmount:C}")
                        .Bold().FontSize(12);
                });
            });
        }

        private void ComposeNotes(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Text("Notes").Bold().FontColor(Colors.Grey.Medium);
                col.Item().Text(data.Notes);
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Page ");
                text.CurrentPageNumber();
                text.Span(" of ");
                text.TotalPages();
            });
        }
    }
}
