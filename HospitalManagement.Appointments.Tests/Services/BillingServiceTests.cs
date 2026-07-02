using AutoMapper;
using HospitalManagement.Appointments.Mappings;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Models.DTOs.Invoice;
using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Patient;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Appointments.Tests.Services
{
    internal class BillingServiceTests
    {
        private Mock<IAppointmentService> appointmentServiceMock;
        private Mock<IPdfGenerator> pdfGeneratorMock;
        private Mock<ILogger<BillingService>> loggerMock;
        private IMapper mapper;
        private BillingService billingService;

        [SetUp]
        public void SetUp()
        {
            appointmentServiceMock = new Mock<IAppointmentService>();
            pdfGeneratorMock = new Mock<IPdfGenerator>();
            loggerMock = new Mock<ILogger<BillingService>>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InvoiceProfile>();
            });
            mapper = config.CreateMapper();
            billingService = new BillingService
            (
                appointmentServiceMock.Object,
                pdfGeneratorMock.Object,
                loggerMock.Object,
                mapper
            );
        }

        [Test]
        public async Task GenerateInvoiceDataAsync_AppointmentNotFound_ReturnFailure()
        {
            int appointmentId = 1;

            appointmentServiceMock.Setup
                (a => a.GetByIdAsync(appointmentId))
                    .ReturnsAsync(Result<AppointmentResponseDto>.Fail(
            $"Appointment with the id {appointmentId} not found", "INVALID_ID"));

            var result = await billingService.GenerateInvoiceAsync(appointmentId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task GenerateInvoiceAsync_AppointmentNotFound_DoesNotCallPdfGenerator()
        {
            int appointmentId = 1;
            var appointment = new AppointmentResponseDto { Id = appointmentId };

            appointmentServiceMock.Setup(
                a => a.GetByIdAsync(appointmentId))
                .ReturnsAsync(Result<AppointmentResponseDto>.Fail(
        $"Appointment with the id {appointmentId} not found", "INVALID_ID"));

            await billingService.GenerateInvoiceAsync(appointmentId);

            pdfGeneratorMock.Verify(p => p.Generate
            (It.IsAny<InvoiceData>()), Times.Never);
        }

        [Test]
        public async Task GenerateInvoiceAsync_AppointmentFound_CallsPdfGenerator()
        {
            int appointmentId = 1;
            var appointment = new AppointmentResponseDto
            {
                Id = appointmentId,
                Patient = new PatientResponseDto { Name = "John", LastName = "Doe" },
                Doctor = new DoctorResponseDto { FirstName = "Jane", LastName = "Smith" },
                Procedures = []
            };

            appointmentServiceMock.Setup(
                a => a.GetByIdAsync(appointmentId))
                .ReturnsAsync(Result<AppointmentResponseDto>.Ok(appointment));
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
            var appointment = new AppointmentResponseDto
            {
                Id = appointmentId,
                Patient = new PatientResponseDto { Name = "John", LastName = "Doe" },
                Doctor = new DoctorResponseDto { FirstName = "Jane", LastName = "Smith" },
                Procedures = []
            };

            appointmentServiceMock.Setup(
                a => a.GetByIdAsync(appointmentId)).ReturnsAsync(Result<AppointmentResponseDto>.Ok(appointment));
            pdfGeneratorMock.Setup(p => p.Generate(It.IsAny<InvoiceData>())).Returns(new byte[5]);

            var result = await billingService.GenerateInvoiceAsync(appointmentId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.PdfBytes, Has.Length.EqualTo(5));
        }

        [Test]
        public async Task GenerateInvoiceAsync_AppointmentFound_MapsInvoiceDataCorrectly()
        {
            int appointmentId = 1;
            var appointment = new AppointmentResponseDto
            {
                Id = 1,
                DateTime = new DateTime(2024, 6, 15, 10, 0, 0),
                Duration = TimeSpan.FromMinutes(30),
                Notes = "Follow up required",
                Patient = new PatientResponseDto { Name = "John", LastName = "Doe" },
                Doctor = new DoctorResponseDto { FirstName = "Jane", LastName = "Smith" },
                Procedures =
                [
                    new AppointmentProcedureResponseDto { ProcedureName = "Blood Test", ProcedurePrice = 50 },
                    new AppointmentProcedureResponseDto { ProcedureName = "X-Ray", ProcedurePrice = 80 }
                ],
                Discount = 10,
                TotalCost = 120
            };

            appointmentServiceMock
                .Setup(a => a.GetByIdAsync(appointmentId))
                .ReturnsAsync(Result<AppointmentResponseDto>.Ok(appointment));

            InvoiceData? capturedData = null;
            pdfGeneratorMock
                .Setup(p => p.Generate(It.IsAny<InvoiceData>()))
                .Callback<InvoiceData>(data => capturedData = data)
                .Returns(new byte[5]);

            await billingService.GenerateInvoiceAsync(appointmentId);

            Assert.That(capturedData.PatientName, Is.EqualTo("John Doe"));
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
            var appointment = new AppointmentResponseDto
            {
                Id = appointmentId,
                Patient = null,
                Doctor = null
            };

            appointmentServiceMock
                .Setup(a => a.GetByIdAsync(appointmentId))
                .ReturnsAsync(Result<AppointmentResponseDto>.Ok(appointment));

            var result = await billingService.GenerateInvoiceAsync(appointmentId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DATA"));
        }
    }
}
