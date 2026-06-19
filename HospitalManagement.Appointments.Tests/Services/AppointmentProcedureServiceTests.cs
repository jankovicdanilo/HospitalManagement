using AutoMapper;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Appointments.Services.Validations;
using HospitalManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.Appointments.Tests.Services
{
    [TestFixture]
    internal class AppointmentProcedureServiceTests
    {
        private Mock<IAppointmentProcedureRepository> appointmentProcedureRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<IAppointmentProcedureValidation> appointmentProcedureValidationMock;
        private Mock<ILogger<AppointmentProcedureService>> loggerMock;
        private AppointmentProcedureService appointmentProcedureService;

        [SetUp]
        public void SetUp()
        {
            appointmentProcedureRepositoryMock = new Mock<IAppointmentProcedureRepository>();
            mapperMock = new Mock<IMapper>();
            appointmentProcedureValidationMock = new Mock<IAppointmentProcedureValidation>();
            loggerMock = new Mock<ILogger<AppointmentProcedureService>>();

            appointmentProcedureService = new AppointmentProcedureService(
                appointmentProcedureRepositoryMock.Object,
                mapperMock.Object,
                appointmentProcedureValidationMock.Object,
                loggerMock.Object
            );
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            int procedureId = 1;
            var request = new AppointmentProcedureCreateRequestDto { AppointmentId = appointmentId, ProcedureId = procedureId };
            var appointmentProcedure = new AppointmentProcedure { AppointmentId = appointmentId, ProcedureId = procedureId };
            var appointmentProcedureDto = new AppointmentProcedureCreateResponseDto { AppointmentId = appointmentId, ProcedureId = procedureId };

            appointmentProcedureValidationMock.Setup(v => v.ValidateForCreate(request.AppointmentId, request.ProcedureId)).ReturnsAsync(Result.Ok("Validation ok"));
            mapperMock.Setup(m => m.Map<AppointmentProcedure>(request)).Returns(appointmentProcedure);
            appointmentProcedureRepositoryMock.Setup(r => r.CreateAsync(appointmentProcedure)).ReturnsAsync(appointmentProcedure);
            mapperMock.Setup(m => m.Map<AppointmentProcedureCreateResponseDto>(appointmentProcedure)).Returns(appointmentProcedureDto);

            var result = await appointmentProcedureService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(appointmentProcedureDto));
        }

        [Test]
        public async Task CreateAsync_ValidationFails_ReturnsFailure()
        {
            int appointmentId = 1;
            int procedureId = 1;
            var request = new AppointmentProcedureCreateRequestDto { AppointmentId = appointmentId, ProcedureId = procedureId };

            appointmentProcedureValidationMock.Setup(v => v.ValidateForCreate(request.AppointmentId, request.ProcedureId))
                .ReturnsAsync(Result.Fail("Appointment not found", "INVALID_APPOINTMENT_ID"));

            var result = await appointmentProcedureService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_APPOINTMENT_ID"));
        }

        [Test]
        public async Task GetByAppointmentAndProcedureIdAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            int procedureId = 1;
            var appointmentProcedure = new AppointmentProcedure { AppointmentId = appointmentId, ProcedureId = procedureId };
            var appointmentProcedureDto = new AppointmentProcedureResponseDto { AppointmentId = appointmentId, ProcedureId = procedureId };

            appointmentProcedureValidationMock.Setup(v => v.ValidateForGet(appointmentId, procedureId)).ReturnsAsync(Result.Ok("Validation ok"));
            appointmentProcedureRepositoryMock.Setup(r => r.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId)).ReturnsAsync(appointmentProcedure);
            mapperMock.Setup(m => m.Map<AppointmentProcedureResponseDto>(appointmentProcedure)).Returns(appointmentProcedureDto);

            var result = await appointmentProcedureService.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(appointmentProcedureDto));
        }

        [Test]
        public async Task GetByAppointmentAndProcedureIdAsync_ValidationFails_ReturnsFailure()
        {
            int appointmentId = 1;
            int procedureId = 1;

            appointmentProcedureValidationMock.Setup(v => v.ValidateForGet(appointmentId, procedureId))
                .ReturnsAsync(Result.Fail("Appointment not found", "INVALID_APPOINTMENT_ID"));

            var result = await appointmentProcedureService.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_APPOINTMENT_ID"));
        }

        [Test]
        public async Task GetByAppointmentAndProcedureIdAsync_NotLinked_ReturnsFailure()
        {
            int appointmentId = 1;
            int procedureId = 1;

            appointmentProcedureValidationMock.Setup(v => v.ValidateForGet(appointmentId, procedureId)).ReturnsAsync(Result.Ok("Validation ok"));
            appointmentProcedureRepositoryMock.Setup(r => r.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId)).ReturnsAsync((AppointmentProcedure?)null);

            var result = await appointmentProcedureService.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task DeleteAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            int procedureId = 1;
            var appointmentProcedure = new AppointmentProcedure { AppointmentId = appointmentId, ProcedureId = procedureId };
            var appointmentProcedureDto = new AppointmentProcedureResponseDto { AppointmentId = appointmentId, ProcedureId = procedureId };

            appointmentProcedureValidationMock.Setup(v => v.ValidateForDelete(appointmentId, procedureId)).ReturnsAsync(Result.Ok("Validation ok"));
            appointmentProcedureRepositoryMock.Setup(r => r.DeleteAsync(appointmentId, procedureId)).ReturnsAsync(appointmentProcedure);
            mapperMock.Setup(m => m.Map<AppointmentProcedureResponseDto>(appointmentProcedure)).Returns(appointmentProcedureDto);

            var result = await appointmentProcedureService.DeleteAsync(appointmentId, procedureId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(appointmentProcedureDto));
        }

        [Test]
        public async Task DeleteAsync_ValidationFails_ReturnsFailure()
        {
            int appointmentId = 1;
            int procedureId = 1;

            appointmentProcedureValidationMock.Setup(v => v.ValidateForDelete(appointmentId, procedureId))
                .ReturnsAsync(Result.Fail($"Appointment with id {appointmentId} not found", "INVALID_APPOINTMENT_ID"));

            var result = await appointmentProcedureService.DeleteAsync(appointmentId, procedureId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_APPOINTMENT_ID"));
        }

        [Test]
        public async Task DeleteAsync_NotLinked_ReturnsFailure()
        {
            int appointmentId = 1;
            int procedureId = 1;

            appointmentProcedureValidationMock.Setup(v => v.ValidateForDelete(appointmentId, procedureId)).ReturnsAsync(Result.Ok("Validation ok"));
            appointmentProcedureRepositoryMock.Setup(r => r.DeleteAsync(appointmentId, procedureId)).ReturnsAsync((AppointmentProcedure?)null);

            var result = await appointmentProcedureService.DeleteAsync(appointmentId, procedureId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }
    }
}