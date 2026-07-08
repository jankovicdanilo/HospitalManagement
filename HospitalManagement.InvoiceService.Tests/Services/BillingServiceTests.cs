using AutoMapper;
using HospitalManagement.InvoiceService.Clients.Interfaces;
using HospitalManagement.InvoiceService.Mappings;
using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.InvoiceService.Models.Enums;
using HospitalManagement.InvoiceService.Services.Implementations;
using HospitalManagement.InvoiceService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.InvoiceService.Tests.Services
{
    [TestFixture]
    internal class BillingServiceTests
    {
        private Mock<IAppointmentServiceClient> appointmentServiceClientMock;
        private Mock<IInvoiceDocumentGeneratorFactory> generatorFactoryMock;
        private Mock<IInvoiceDocumentGenerator> generatorMock;
        private Mock<ILogger<BillingService>> loggerMock;
        private IMapper mapper;
        private BillingService billingService;

        [SetUp]
        public void SetUp()
        {
            appointmentServiceClientMock = new Mock<IAppointmentServiceClient>();
            generatorFactoryMock = new Mock<IInvoiceDocumentGeneratorFactory>();
            generatorMock = new Mock<IInvoiceDocumentGenerator>();
            loggerMock = new Mock<ILogger<BillingService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InvoiceProfile>();
            });
            mapper = config.CreateMapper();

            billingService = new BillingService
            (
                appointmentServiceClientMock.Object,
                generatorFactoryMock.Object,
                loggerMock.Object,
                mapper
            );
        }

        private static AppointmentInvoiceDto CreateValidAppointment(int appointmentId) => new()
        {
            Id = appointmentId,
            Patient = new InvoicePatientDto { Name = "John", LastName = "Doe" },
            Doctor = new InvoiceDoctorDto { FirstName = "Jane", LastName = "Smith" },
            Procedures = []
        };

        [Test]
        public async Task GenerateInvoiceDataAsync_AppointmentNotFound_ReturnFailure()
        {
            int appointmentId = 1;

            appointmentServiceClientMock.Setup
                (a => a.GetAppointmentAsync(appointmentId))
                    .ReturnsAsync((AppointmentInvoiceDto?)null);

            var result = await billingService.GenerateInvoiceAsync(appointmentId, InvoiceFormat.Pdf);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task GenerateInvoiceAsync_AppointmentNotFound_DoesNotCallPdfGenerator()
        {
            int appointmentId = 1;
            var appointment = new AppointmentInvoiceDto { Id = appointmentId };

            appointmentServiceClientMock.Setup(
                a => a.GetAppointmentAsync(appointmentId))
                .ReturnsAsync((AppointmentInvoiceDto?)null);

            await billingService.GenerateInvoiceAsync(appointmentId);

            pdfGeneratorMock.Verify(p => p.Generate
            (It.IsAny<InvoiceData>()), Times.Never);
        }

        [Test]
        public async Task GenerateInvoiceAsync_AppointmentFound_CallsPdfGenerator()
        {
            int appointmentId = 1;
            var appointment = new AppointmentInvoiceDto
            {
                Id = appointmentId,
                Patient = new InvoicePatientDto { Name = "John", LastName = "Doe" },
                Doctor = new InvoiceDoctorDto { FirstName = "Jane", LastName = "Smith" },
                Procedures = []
            };

            appointmentServiceClientMock.Setup(
                a => a.GetAppointmentAsync(appointmentId))
                .ReturnsAsync(appointment);
            pdfGeneratorMock.Setup(
                p => p.Generate(It.IsAny<InvoiceData>()))
                .Returns(new byte[5]);

            var result = await billingService.GenerateInvoiceAsync(appointmentId);

            pdfGeneratorMock.Verify(p => p.Generate(It.IsAny<InvoiceData>()), Times.Once);
        }

        [Test]
        public async Task GenerateInvoiceAsync_AppointmentFound_ReturnPdfBytes()
        {
            int appointmentId = 1;
            var appointment = new AppointmentInvoiceDto
            {
                Id = appointmentId,
                Patient = new InvoicePatientDto { Name = "John", LastName = "Doe" },
                Doctor = new InvoiceDoctorDto { FirstName = "Jane", LastName = "Smith" },
                Procedures = []
            };

            appointmentServiceClientMock.Setup(
                a => a.GetAppointmentAsync(appointmentId)).ReturnsAsync(appointment);
            pdfGeneratorMock.Setup(p => p.Generate(It.IsAny<InvoiceData>())).Returns(new byte[5]);

            var result = await billingService.GenerateInvoiceAsync(appointmentId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.PdfBytes, Has.Length.EqualTo(5));
        }

        [Test]
        public async Task GenerateInvoiceAsync_AppointmentFound_MapsInvoiceDataCorrectly()
        {
            int appointmentId = 1;
            var appointment = new AppointmentInvoiceDto
            {
                Id = 1,
                DateTime = new DateTime(2024, 6, 15, 10, 0, 0),
                Duration = TimeSpan.FromMinutes(30),
                Notes = "Follow up required",
                Patient = new InvoicePatientDto { Name = "John", LastName = "Doe" },
                Doctor = new InvoiceDoctorDto { FirstName = "Jane", LastName = "Smith" },
                Procedures =
                [
                    new InvoiceProcedureDto  { ProcedureName = "Blood Test", ProcedurePrice = 50 },
                    new InvoiceProcedureDto  { ProcedureName = "X-Ray", ProcedurePrice = 80 }
                ],
                Discount = 10,
                TotalCost = 120
            };

            appointmentServiceClientMock
                .Setup(a => a.GetAppointmentAsync(appointmentId))
                .ReturnsAsync(appointment);

            InvoiceData? capturedData = null;
            pdfGeneratorMock
                .Setup(p => p.Generate(It.IsAny<InvoiceData>()))
                .Callback<InvoiceData>(data => capturedData = data)
                .Returns(new byte[5]);

            await billingService.GenerateInvoiceAsync(appointmentId);

            Assert.That(capturedData!.PatientName, Is.EqualTo("John Doe"));
            Assert.That(capturedData.DoctorName, Is.EqualTo("Jane Smith"));
            Assert.That(capturedData.AppointmentDate, Is.EqualTo(appointment.DateTime));
            Assert.That(capturedData.Duration, Is.EqualTo(appointment.Duration));
            Assert.That(capturedData.Notes, Is.EqualTo(appointment.Notes));
            Assert.That(capturedData.Subtotal, Is.EqualTo(130));
            Assert.That(capturedData.Discount, Is.EqualTo(10));
            Assert.That(capturedData.TotalAmount, Is.EqualTo(120));
            Assert.That(capturedData.Procedures.Count, Is.EqualTo(2));
            Assert.That(capturedData.Procedures[0].Name, Is.EqualTo("Blood Test"));
            Assert.That(capturedData.Procedures[1].Name, Is.EqualTo("X-Ray"));
        }

        [Test]
        public async Task GenerateInvoiceAsync_AppointmentHasIncompleteData_ReturnsFailure()
        {
            int appointmentId = 1;
            var appointment = new AppointmentInvoiceDto
            {
                Id = appointmentId,
                Patient = null,
                Doctor = null
            };

            appointmentServiceClientMock
                .Setup(a => a.GetAppointmentAsync(appointmentId))
                .ReturnsAsync(appointment);

            var result = await billingService.GenerateInvoiceAsync(appointmentId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DATA"));
        }
    }
}
