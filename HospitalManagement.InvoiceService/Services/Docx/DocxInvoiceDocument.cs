using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.InvoiceService.Services.Implementations;
using System.Globalization;

namespace HospitalManagement.InvoiceService.Services.Docx
{
    public class DocxInvoiceDocument
    {
        private readonly InvoiceData data;
        private readonly InvoiceLabels labels;

        public DocxInvoiceDocument(InvoiceData data, string language)
        {
            this.data = data;
            this.labels = InvoiceLabels.Get(language);
        }

        public void Compose(MainDocumentPart mainPart)
        {
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            ComposeHeader(body);
            ComposePatientInfo(body);
            ComposeProceduresTable(body);
            ComposeTotals(body);

            if (!string.IsNullOrEmpty(data.Notes))
            {
                ComposeNotes(body);
            }
        }

        private void ComposeHeader(Body body)
        {
            body.AppendChild(new Paragraph(
                new Run(new RunProperties(new Bold(), new FontSize { Val = "40" }), new Text(labels.HospitalName))));
            body.AppendChild(new Paragraph(new Run(new Text(labels.Title))));
            body.AppendChild(new Paragraph(
                new Run(new Text($"{labels.Invoice}: {data.InvoiceNumber}    {labels.Date}: {data.IssuedDate:dd/MM/yyyy}"))));
            body.AppendChild(new Paragraph());
        }

        private void ComposePatientInfo(Body body)
        {
            body.AppendChild(new Paragraph(new Run(new RunProperties(new Bold()), new Text($"{labels.Patient}: {data.PatientName}"))));
            body.AppendChild(new Paragraph(new Run(new RunProperties(new Bold()), new Text($"{labels.Doctor}: {data.DoctorName}"))));
            body.AppendChild(new Paragraph(new Run(new Text($"{labels.AppointmentDate}: {data.AppointmentDate:dd/MM/yyyy HH:mm}"))));
            body.AppendChild(new Paragraph(new Run(new Text($"{labels.Duration}: {data.Duration.TotalMinutes} min"))));
            body.AppendChild(new Paragraph(new Run(new RunProperties(new Bold()), new Text($"{labels.Status}: {labels.StatusText(data.Status.ToString())}"))));
            body.AppendChild(new Paragraph());
        }

        private void ComposeProceduresTable(Body body)
        {
            var table = new Table();
            table.AppendChild(new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 6 },
                    new BottomBorder { Val = BorderValues.Single, Size = 6 },
                    new LeftBorder { Val = BorderValues.Single, Size = 6 },
                    new RightBorder { Val = BorderValues.Single, Size = 6 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                )));

            var headerRow = new TableRow();
            headerRow.Append(CreateCell(labels.Procedure, bold: true), CreateCell(labels.Price, bold: true));
            table.Append(headerRow);

            foreach (var item in data.Procedures)
            {
                var row = new TableRow();
                row.Append(CreateCell(item.Name ?? string.Empty), CreateCell($"{item.Price:C}"));
                table.Append(row);
            }

            body.AppendChild(table);
            body.AppendChild(new Paragraph());
        }

        private static TableCell CreateCell(string text, bool bold = false)
        {
            var runProperties = bold ? new RunProperties(new Bold()) : new RunProperties();
            return new TableCell(new Paragraph(new Run(runProperties, new Text(text))));
        }

        private void ComposeTotals(Body body)
        {
            body.AppendChild(new Paragraph(new Run(new Text($"{labels.Subtotal}: {data.Subtotal:C}"))));
            body.AppendChild(new Paragraph(new Run(new Text($"{labels.Discount}: -{data.Discount:C}"))));
            body.AppendChild(new Paragraph(
                new Run(new RunProperties(new Bold(), new FontSize { Val = "24" }), new Text($"{labels.Total}: {data.TotalAmount:C}"))));
        }

        private void ComposeNotes(Body body)
        {
            body.AppendChild(new Paragraph());
            body.AppendChild(new Paragraph(new Run(new RunProperties(new Bold()), new Text(labels.Notes))));
            body.AppendChild(new Paragraph(new Run(new Text(data.Notes ?? string.Empty))));
        }
    }
}
