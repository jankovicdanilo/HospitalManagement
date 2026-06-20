using AutoMapper;
using HospitalManagement.Appointments.Clients.Interfaces;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Calculators.Interfaces;
using HospitalManagement.Appointments.Services.Calculators.Results;
using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Appointments.Services.Validations;
using HospitalManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace HospitalManagement.Appointments.Tests.Services
{
    [TestFixture]
    internal class AppointmentServiceTests
    {
        private Mock<IAppointmentRepository> appointmentRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<IAppointmentValidation> appointmentValidationMock;
        private Mock<ILogger<AppointmentService>> loggerMock;
        private Mock<IHospitalManagementClient> mainApiClientMock;
        private IOptions<AppointmentSettings> appointmentSettings;
        private Mock<IAppointmentDiscountCalculator> appointmentDiscountCalculatorMock;
        private AppointmentService appointmentService;

        [SetUp]
        public void SetUp()
        {
            appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            mapperMock = new Mock<IMapper>();
            appointmentValidationMock = new Mock<IAppointmentValidation>();
            loggerMock = new Mock<ILogger<AppointmentService>>();
            mainApiClientMock = new Mock<IHospitalManagementClient>();
            appointmentDiscountCalculatorMock = new Mock<IAppointmentDiscountCalculator>();
            appointmentSettings = Options.Create(new AppointmentSettings { SlotSizeMinutes = 30 });

            appointmentService = new AppointmentService(
                appointmentRepositoryMock.Object,
                mapperMock.Object,
                appointmentValidationMock.Object,
                loggerMock.Object,
                appointmentSettings,
                appointmentDiscountCalculatorMock.Object
            );
        }

        [Test]
        public async Task GetByIdAsync_AppointmentExists_ReturnsSuccess()
        {
            int appointmentId = 1;
            var appointment = new Appointment { Id = appointmentId };
            var appointmentDto = new AppointmentResponseDto { Id = appointmentId };

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);
            mapperMock.Setup(m => m.Map<AppointmentResponseDto>(appointment)).Returns(appointmentDto);

            var result = await appointmentService.GetByIdAsync(appointmentId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(appointmentDto));
        }

        [Test]
        public async Task GetByIdAsync_AppointmentNotFound_ReturnsFailure()
        {
            int appointmentId = 1;

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync((Appointment?)null);

            var result = await appointmentService.GetByIdAsync(appointmentId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task GetByIdAsync_AppointmentPending_DoesNotApplyDiscount()
        {
            int appointmentId = 1;
            var appointment = new Appointment
            {
                Id = appointmentId,
                Status = AppointmentStatus.Pending,
                AppointmentProcedures = new List<AppointmentProcedure>
                {
                    new AppointmentProcedure { ProcedurePrice = 100m }
                }
            };
            var appointmentDto = new AppointmentResponseDto { Id = appointmentId };

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);
            mapperMock.Setup(m => m.Map<AppointmentResponseDto>(appointment)).Returns(appointmentDto);

            var result = await appointmentService.GetByIdAsync(appointmentId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.TotalCost, Is.EqualTo(100m));
            Assert.That(result.Data.Discount, Is.EqualTo(0));
            appointmentDiscountCalculatorMock.Verify(c => c.Calculate(It.IsAny<ICollection<AppointmentProcedure>>()), Times.Never);
        }

        [Test]
        public async Task GetByIdAsync_AppointmentCompleted_AppliesDiscount()
        {
            int appointmentId = 1;
            var procedures = new List<AppointmentProcedure>
            {
                new AppointmentProcedure { ProcedurePrice = 100m }
            };
            var appointment = new Appointment
            {
                Id = appointmentId,
                Status = AppointmentStatus.Completed,
                AppointmentProcedures = procedures
            };
            var appointmentDto = new AppointmentResponseDto { Id = appointmentId };
            var discountResult = new DiscountResult(90m, 10m);

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);
            mapperMock.Setup(m => m.Map<AppointmentResponseDto>(appointment)).Returns(appointmentDto);
            appointmentDiscountCalculatorMock.Setup(c => c.Calculate(procedures)).Returns(discountResult);

            var result = await appointmentService.GetByIdAsync(appointmentId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.TotalCost, Is.EqualTo(90m));
            Assert.That(result.Data.Discount, Is.EqualTo(10m));
            appointmentDiscountCalculatorMock.Verify(c => c.Calculate(procedures), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            int doctorId = 1;
            int patientId = 1;
            var request = new AppointmentCreateRequestDto { PatientId = patientId, DoctorId = doctorId };
            var appointment = new Appointment { Id = appointmentId };
            var appointmentDto = new AppointmentCreateResponseDto { Id = appointmentId };

            appointmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            mapperMock.Setup(m => m.Map<Appointment>(request)).Returns(appointment);
            appointmentRepositoryMock.Setup(r => r.CreateAsync(appointment)).ReturnsAsync(appointment);
            mapperMock.Setup(m => m.Map<AppointmentCreateResponseDto>(appointment)).Returns(appointmentDto);

            var result = await appointmentService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(appointmentDto));
        }

        [Test]
        public async Task CreateAsync_ValidationFails_ReturnsFailure()
        {
            int doctorId = 1;
            int patientId = 1;
            var request = new AppointmentCreateRequestDto { PatientId = patientId, DoctorId = doctorId };

            appointmentValidationMock.Setup(v => v.ValidateAll(request))
                .ReturnsAsync(Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID"));

            var result = await appointmentService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DOCTOR_ID"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            var request = new AppointmentUpdateRequestDto { Id = appointmentId, DoctorId = 1, PatientId = 1 };
            var appointment = new Appointment { Id = appointmentId };
            var appointmentDto = new AppointmentUpdateResponseDto { Id = appointmentId };

            appointmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(appointment);
            appointmentRepositoryMock.Setup(r => r.UpdateAsync(appointment)).ReturnsAsync(appointment);
            mapperMock.Setup(m => m.Map<AppointmentUpdateResponseDto>(appointment)).Returns(appointmentDto);

            var result = await appointmentService.UpdateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(appointmentDto));
        }

        [Test]
        public async Task UpdateAsync_ValidationFails_ReturnsFailure()
        {
            int appointmentId = 1;
            var request = new AppointmentUpdateRequestDto { Id = appointmentId };

            appointmentValidationMock.Setup(v => v.ValidateAll(request))
                .ReturnsAsync(Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID"));

            var result = await appointmentService.UpdateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DOCTOR_ID"));
        }

        [Test]
        public async Task UpdateAsync_AppointmentNotFound_ReturnsFailure()
        {
            int appointmentId = 1;
            var request = new AppointmentUpdateRequestDto { Id = appointmentId };

            appointmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((Appointment?)null);

            var result = await appointmentService.UpdateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task UpdateAsync_AppointmentNotPending_ReturnsFailure()
        {
            int appointmentId = 1;
            var request = new AppointmentUpdateRequestDto { Id = appointmentId };
            var appointment = new Appointment { Id = appointmentId, Status = AppointmentStatus.Completed };

            appointmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(appointment);

            var result = await appointmentService.UpdateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_STATUS"));
        }

        [Test]
        public async Task Delete_AppointmentExists_ReturnsSuccess()
        {
            int appointmentId = 1;
            var appointment = new Appointment { Id = appointmentId };

            appointmentRepositoryMock.Setup(r => r.Delete(appointmentId)).ReturnsAsync(appointment);

            var result = await appointmentService.Delete(appointmentId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Appointment deleted"));
        }

        [Test]
        public async Task Delete_AppointmentNotFound_ReturnsFailure()
        {
            int appointmentId = 1;

            appointmentRepositoryMock.Setup(r => r.Delete(appointmentId)).ReturnsAsync((Appointment?)null);

            var result = await appointmentService.Delete(appointmentId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task GetFreeSlotsAsync_ReturnsNotImplemented()
        {
            // GetFreeSlotsAsync is temporarily stubbed pending IMainApiClient wiring
            var result = await appointmentService.GetFreeSlotsAsync(1, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("NOT_IMPLEMENTED"));
        }

        [Test]
        public async Task UpdateStatusAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            var request = new AppointmentStatusUpdateDto { Id = appointmentId, Status = AppointmentStatus.Pending };
            var appointment = new Appointment { Id = appointmentId };

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);
            appointmentRepositoryMock.Setup(r => r.UpdateAsync(appointment)).ReturnsAsync(appointment);

            var result = await appointmentService.UpdateStatusAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Appointment status updated"));
        }

        [Test]
        public async Task UpdateStatusAsync_AppointmentNotFound_ReturnsFailure()
        {
            int appointmentId = 1;
            var request = new AppointmentStatusUpdateDto { Id = appointmentId };

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync((Appointment?)null);

            var result = await appointmentService.UpdateStatusAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task UpdateStatusAsync_AppointmentNotPending_ReturnsFailure()
        {
            int appointmentId = 1;
            var request = new AppointmentStatusUpdateDto { Id = appointmentId, Status = AppointmentStatus.Completed };
            var appointment = new Appointment { Id = appointmentId, Status = AppointmentStatus.Completed };

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

            var result = await appointmentService.UpdateStatusAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_STATUS"));
        }
    }
}