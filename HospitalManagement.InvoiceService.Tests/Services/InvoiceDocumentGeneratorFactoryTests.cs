using HospitalManagement.InvoiceService.Models.Enums;
using HospitalManagement.InvoiceService.Services.Docx;
using HospitalManagement.InvoiceService.Services.Implementations;
using HospitalManagement.InvoiceService.Services.Interfaces;
using HospitalManagement.InvoiceService.Services.Pdf;

namespace HospitalManagement.InvoiceService.Tests.Services
{
    [TestFixture]
    internal class InvoiceDocumentGeneratorFactoryTests
    {
        [Test]
        public void GetGenerator_PdfFormat_ReturnsPdfInvoiceGenerator()
        {
            var generators = new List<IInvoiceDocumentGenerator> { new PdfInvoiceGenerator(), new DocxInvoiceGenerator() };
            var factory = new InvoiceDocumentGeneratorFactory(generators);

            var result = factory.GetGenerator(InvoiceFormat.Pdf);

            Assert.That(result, Is.InstanceOf<PdfInvoiceGenerator>());
        }

        [Test]
        public void GetGenerator_DocxFormat_ReturnsDocxInvoiceGenerator()
        {
            var generators = new List<IInvoiceDocumentGenerator> { new PdfInvoiceGenerator(), new DocxInvoiceGenerator() };
            var factory = new InvoiceDocumentGeneratorFactory(generators);

            var result = factory.GetGenerator(InvoiceFormat.Docx);

            Assert.That(result, Is.InstanceOf<DocxInvoiceGenerator>());
        }

        [Test]
        public void GetGenerator_UnsupportedFormat_ThrowsArgumentException()
        {
            var generators = new List<IInvoiceDocumentGenerator> { new PdfInvoiceGenerator(), new DocxInvoiceGenerator() };
            var factory = new InvoiceDocumentGeneratorFactory(generators);

            Assert.Throws<ArgumentException>(() => factory.GetGenerator((InvoiceFormat)999));
        }
    }
}
