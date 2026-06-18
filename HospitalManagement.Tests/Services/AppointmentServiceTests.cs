using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Models.Enums;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Calculators.Interfaces;
using HospitalManagement.Services.Calculators.Results;
using HospitalManagement.Services.Implementations;
using HospitalManagement.Services.Validations;
using HospitalManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace HospitalManagement.Tests.Services
{
    [TestFixture]
    internal class AppointmentServiceTests
    {
        private Mock<IAppointmentRepository> appointmentRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<IAppointmentValidation> appointmentValidationMock;
        private Mock<ILogger<AppointmentService>> loggerMock;
        private Mock<IPatientRepository> patientRepositoryMock;
        private Mock<IDoctorRepository> doctorRepositoryMock;
        private Mock<IDoctorScheduleRepository> doctorScheduleRepositoryMock;
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
            patientRepositoryMock = new Mock<IPatientRepository>();
            doctorRepositoryMock = new Mock<IDoctorRepository>();
            doctorScheduleRepositoryMock = new Mock<IDoctorScheduleRepository>();
            appointmentDiscountCalculatorMock = new Mock<IAppointmentDiscountCalculator>();
            appointmentSettings = Options.Create(new AppointmentSettings { SlotSizeMinutes = 30 });

            appointmentService = new AppointmentService
                (
                    appointmentRepositoryMock.Object,
                    mapperMock.Object,
                    appointmentValidationMock.Object,
                    loggerMock.Object,
                    patientRepositoryMock.Object,
                    doctorRepositoryMock.Object,
                    doctorScheduleRepositoryMock.Object,
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
            var procedure = new Procedure { Id = 1, Price = 100m };
            var appointment = new Appointment
            {
                Id = appointmentId,
                Status = AppointmentStatus.Pending,
                AppointmentProcedures = new List<AppointmentProcedure>
        {
            new AppointmentProcedure { Procedure = procedure }
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
            var procedure = new Procedure { Id = 1, Price = 100m };
            var procedures = new List<AppointmentProcedure>
    {
        new AppointmentProcedure { Procedure = procedure }
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
            var appointment = new Appointment {Id = appointmentId};
            var appointmentDto = new AppointmentCreateResponseDto {  Id = appointmentId };
            var doctor = new Doctor { Id = 1, FirstName = "John", LastName = "Doe" };
            var patient = new Patient { Id = 1, Email = "test@test.com" };

            appointmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            mapperMock.Setup(m => m.Map<Appointment>(request)).Returns(appointment);
            appointmentRepositoryMock.Setup(r => r.CreateAsync(appointment)).ReturnsAsync(appointment);
            patientRepositoryMock.Setup(p => p.GetByIdAsync(patientId)).ReturnsAsync(patient);
            doctorRepositoryMock.Setup(d => d.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
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
            int doctorId = 1;
            int patientId = 1;
            var request = new AppointmentUpdateRequestDto { Id = appointmentId,DoctorId = doctorId , PatientId = patientId};
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
            AppointmentStatus appointmentStatus = AppointmentStatus.Completed;
            var request = new AppointmentUpdateRequestDto { Id = appointmentId };
            var appointment = new Appointment { Id = appointmentId , Status = appointmentStatus };

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
        public async Task GetFreeSlotsAsync_ReturnsSuccess()
        {
            int id = 1;
            int doctorId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var doctorSchedule = new DoctorSchedule { Id = id, DoctorId = doctorId, StartHour = 8, EndHour = 16 };


            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(doctorId, date.DayOfWeek)).ReturnsAsync(doctorSchedule);
            appointmentRepositoryMock
                .Setup(r => r.GetByDoctorIdAndDateAsync(doctorId, date))
                .ReturnsAsync(new List<Appointment>());

            var result = await appointmentService.GetFreeSlotsAsync(doctorId, date);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Empty);
        }

        [Test]
        public async Task GetFreeSlotsAsync_DoctorNotAvailable_ReturnsFailure()
        {
            int doctorId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(doctorId, date.DayOfWeek)).ReturnsAsync((DoctorSchedule?)null);

            var result = await appointmentService.GetFreeSlotsAsync(doctorId, date);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("DOCTOR_NOT_AVAILABLE"));
        }

        [Test]
        public async Task GetFreeSlotsAsync_PastDate_ReturnsFailure()
        {
            int id = 1;
            int doctorId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            var doctorSchedule = new DoctorSchedule { Id = id, DoctorId = doctorId };


            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(doctorId, date.DayOfWeek)).ReturnsAsync(doctorSchedule);

            var result = await appointmentService.GetFreeSlotsAsync(doctorId, date);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DATE"));
        }

        [Test]
        public async Task UpdateStatusAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            AppointmentStatus appointmentStatus = AppointmentStatus.Pending;
            var request = new AppointmentStatusUpdateDto { Id = appointmentId, Status = appointmentStatus };
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
            AppointmentStatus appointmentStatus = AppointmentStatus.Completed;
            var request = new AppointmentStatusUpdateDto { Id = appointmentId, Status = appointmentStatus };
            var appointment = new Appointment { Id = appointmentId, Status = appointmentStatus };

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

            var result = await appointmentService.UpdateStatusAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_STATUS"));
        }
    }
}
