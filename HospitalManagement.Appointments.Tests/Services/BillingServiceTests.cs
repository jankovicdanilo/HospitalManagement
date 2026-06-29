using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.DTOs.Invoice;
using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Shared.Common;
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
        private BillingService billingService;

        [SetUp]
        public void SetUp()
        {
            appointmentServiceMock = new Mock<IAppointmentService>();
            pdfGeneratorMock = new Mock<IPdfGenerator>();
            loggerMock = new Mock<ILogger<BillingService>>();
            billingService = new BillingService
            (
                appointmentServiceMock.Object,
                pdfGeneratorMock.Object,
                loggerMock.Object
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
            var appointment = new AppointmentResponseDto { Id = appointmentId };

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
            var appointment = new AppointmentResponseDto { Id = appointmentId };

            appointmentServiceMock.Setup(
                a => a.GetByIdAsync(appointmentId)).ReturnsAsync(Result<AppointmentResponseDto>.Ok(appointment));
            pdfGeneratorMock.Setup(p => p.Generate(It.IsAny<InvoiceData>())).Returns(new byte[5]);

            var result = await billingService.GenerateInvoiceAsync(appointmentId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Has.Length.EqualTo(5));
        }
    }
}
