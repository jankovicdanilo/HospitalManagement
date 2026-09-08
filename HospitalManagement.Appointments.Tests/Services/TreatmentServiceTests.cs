using AutoMapper;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.DTOs.Treatment;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Appointments.Services.Validations;
using HospitalManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Moq;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.Appointments.Tests.Services
{
    [TestFixture]
    internal class TreatmentServiceTests
    {
        private Mock<ITreatmentRepository> treatmentRepositoryMock;
        private Mock<ITreatmentValidation> treatmentValidationMock;
        private Mock<IAppointmentService> appointmentServiceMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<TreatmentService>> loggerMock;
        private TreatmentService treatmentService;

        [SetUp]
        public void SetUp()
        {
            treatmentRepositoryMock = new Mock<ITreatmentRepository>();
            treatmentValidationMock = new Mock<ITreatmentValidation>();
            appointmentServiceMock = new Mock<IAppointmentService>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<TreatmentService>>();

            treatmentService = new TreatmentService(
                treatmentRepositoryMock.Object,
                appointmentServiceMock.Object,
                mapperMock.Object,
                treatmentValidationMock.Object,
                loggerMock.Object
            );
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            int patientId = 5;
            var request = new TreatmentCreateRequestDto { AppointmentId = appointmentId };
            var treatment = new Treatment { AppointmentId = appointmentId };
            var treatmentDto = new TreatmentCreateResponseDto { AppointmentId = appointmentId };
            var appointmentDto = new AppointmentResponseDto { Id = appointmentId, Patient = new PatientResponseDto { Id = patientId } };

            treatmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            mapperMock.Setup(m => m.Map<Treatment>(request)).Returns(treatment);
            treatmentRepositoryMock.Setup(r => r.CreateAsync(treatment)).ReturnsAsync(treatment);
            mapperMock.Setup(m => m.Map<TreatmentCreateResponseDto>(treatment)).Returns(treatmentDto);
            appointmentServiceMock.Setup(s => s.GetByIdAsync(appointmentId)).ReturnsAsync(Result<AppointmentResponseDto>.Ok(appointmentDto));

            var result = await treatmentService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(treatmentDto));
        }

        [Test]
        public async Task CreateAsync_ValidationFails_ReturnsFailure()
        {
            int appointmentId = 1;
            var request = new TreatmentCreateRequestDto { AppointmentId = appointmentId };

            treatmentValidationMock.Setup(v => v.ValidateAll(request))
                .ReturnsAsync(Result.Fail(
                    $"Appointment with id {request.AppointmentId} not found",
                    "APPOINTMENT_NOT_FOUND"));

            var result = await treatmentService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("APPOINTMENT_NOT_FOUND"));
            appointmentServiceMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task CreateAsync_ValidRequest_InvalidatesPatientSummaryCache()
        {
            int appointmentId = 1;
            int patientId = 5;
            var request = new TreatmentCreateRequestDto { AppointmentId = appointmentId };
            var treatment = new Treatment { AppointmentId = appointmentId };
            var treatmentDto = new TreatmentCreateResponseDto { AppointmentId = appointmentId };
            var appointmentDto = new AppointmentResponseDto{Id = appointmentId,Patient = new PatientResponseDto { Id = patientId }
            };

            treatmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            mapperMock.Setup(m => m.Map<Treatment>(request)).Returns(treatment);
            treatmentRepositoryMock.Setup(t => t.CreateAsync(treatment)).ReturnsAsync(treatment);
            mapperMock.Setup(m => m.Map<TreatmentCreateResponseDto>(treatment)).Returns(treatmentDto);
            appointmentServiceMock.Setup(a => a.GetByIdAsync(appointmentId)).ReturnsAsync(Result<AppointmentResponseDto>.Ok(appointmentDto));

            await treatmentService.CreateAsync(request);

            appointmentServiceMock.Verify(s => s.InvalidatePatientSummaryCacheAsync(patientId), Times.Once);
        }

        [Test]
        public async Task CreateAsync_AppointmentLookupFails_StillReturnsSuccessButDoesNotInvalidateCache()
        {
            int appointmentId = 1;
            var request = new TreatmentCreateRequestDto { AppointmentId = appointmentId };
            var treatment = new Treatment { AppointmentId = appointmentId };
            var treatmentDto = new TreatmentCreateResponseDto { AppointmentId = appointmentId };

            treatmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            mapperMock.Setup(m => m.Map<Treatment>(request)).Returns(treatment);
            treatmentRepositoryMock.Setup(t => t.CreateAsync(treatment)).ReturnsAsync(treatment);
            mapperMock.Setup(m => m.Map<TreatmentCreateResponseDto>(treatment)).Returns(treatmentDto);
            appointmentServiceMock.Setup(a => a.GetByIdAsync(appointmentId))
                .ReturnsAsync(Result<AppointmentResponseDto>.Fail("Not found", "INVALID_ID", ErrorType.NotFound));

            var result = await treatmentService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            appointmentServiceMock.Verify(a => a.InvalidatePatientSummaryCacheAsync(It.IsAny<int>()), Times.Never);
        }
    }
}