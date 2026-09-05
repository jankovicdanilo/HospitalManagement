using AutoMapper;
using HospitalManagement.Appointments.Clients.Interfaces;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Calculators.Interfaces;
using HospitalManagement.Appointments.Services.Calculators.Results;
using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Appointments.Services.Validations;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.Shared.Models.DTOs.Patient;
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
        private Mock<IQueryServiceClient> queryServiceClientMock;
        private IOptions<AppointmentSettings> appointmentSettings;
        private Mock<IAppointmentDiscountCalculator> appointmentDiscountCalculatorMock;
        private Mock<IClinicTimeZoneProvider> clinicTimeZoneProviderMock;
        private AppointmentService appointmentService;

        [SetUp]
        public void SetUp()
        {
            appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            mapperMock = new Mock<IMapper>();
            appointmentValidationMock = new Mock<IAppointmentValidation>();
            loggerMock = new Mock<ILogger<AppointmentService>>();
            queryServiceClientMock = new Mock<IQueryServiceClient>();
            appointmentDiscountCalculatorMock = new Mock<IAppointmentDiscountCalculator>();
            appointmentSettings = Options.Create(new AppointmentSettings { SlotSizeMinutes = 30 });
            clinicTimeZoneProviderMock = new Mock<IClinicTimeZoneProvider>();

            clinicTimeZoneProviderMock.Setup(c => c.ToLocal(It.IsAny<DateTime>())).Returns((DateTime dt) => dt);
            clinicTimeZoneProviderMock.Setup(c => c.ToUtc(It.IsAny<DateTime>())).Returns((DateTime dt) => dt);

            appointmentService = new AppointmentService(
                appointmentRepositoryMock.Object,
                mapperMock.Object,
                appointmentValidationMock.Object,
                loggerMock.Object,
                appointmentSettings,
                appointmentDiscountCalculatorMock.Object,
                queryServiceClientMock.Object,
                clinicTimeZoneProviderMock.Object
            );
        }

        [Test]
        public async Task GetAllAsync_ReturnsSuccessWithEnrichedNames()
        {
            var filter = new AppointmentFilterDto { PageNumber = 1, PageSize = 20 };
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, DoctorId = 5, PatientId = 10, AppointmentProcedures = new List<AppointmentProcedure>() }
            };
            var dtos = new List<AppointmentListResponseDto> { new AppointmentListResponseDto { Id = 1, DoctorId = 5, PatientId = 10 } };

            appointmentRepositoryMock.Setup(r => r.GetAllAsync(filter)).ReturnsAsync((appointments, 1));
            mapperMock.Setup(m => m.Map<List<AppointmentListResponseDto>>(appointments)).Returns(dtos);
            queryServiceClientMock.Setup(c => c.GetDoctorAsync(5))
                .ReturnsAsync(new DoctorResponseDto { Id = 5, FirstName = "Ana", LastName = "Kovac" });
            queryServiceClientMock.Setup(c => c.GetPatientAsync(10))
                .ReturnsAsync(new PatientResponseDto { Id = 10, Name = "Marko", LastName = "Petrovic" });

            var result = await appointmentService.GetAllAsync(filter);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Items[0].DoctorName, Is.EqualTo("Ana Kovac"));
            Assert.That(result.Data.Items[0].PatientName, Is.EqualTo("Marko Petrovic"));
        }

        [Test]
        public async Task GetAllAsync_SameDoctorOnMultipleAppointments_CallsDoctorLookupOnlyOnce()
        {
            var filter = new AppointmentFilterDto { PageNumber = 1, PageSize = 20 };
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, DoctorId = 5, PatientId = 10, AppointmentProcedures = new List<AppointmentProcedure>() },
                new Appointment { Id = 2, DoctorId = 5, PatientId = 11, AppointmentProcedures = new List<AppointmentProcedure>() }
            };
            var dtos = new List<AppointmentListResponseDto>
            {
                new AppointmentListResponseDto { Id = 1, DoctorId = 5, PatientId = 10 },
                new AppointmentListResponseDto { Id = 2, DoctorId = 5, PatientId = 11 }
            };

            appointmentRepositoryMock.Setup(r => r.GetAllAsync(filter)).ReturnsAsync((appointments, 2));
            mapperMock.Setup(m => m.Map<List<AppointmentListResponseDto>>(appointments)).Returns(dtos);
            queryServiceClientMock.Setup(c => c.GetDoctorAsync(5))
                .ReturnsAsync(new DoctorResponseDto { Id = 5, FirstName = "Ana", LastName = "Kovac" });
            queryServiceClientMock.Setup(c => c.GetPatientAsync(It.IsAny<int>()))
                .ReturnsAsync(new PatientResponseDto { Name = "Test", LastName = "Patient" });

            var result = await appointmentService.GetAllAsync(filter);

            Assert.That(result.Data!.Items[0].DoctorName, Is.EqualTo("Ana Kovac"));
            Assert.That(result.Data.Items[1].DoctorName, Is.EqualTo("Ana Kovac"));
            queryServiceClientMock.Verify(c => c.GetDoctorAsync(5), Times.Once);
        }

        [Test]
        public async Task GetAllAsync_DoctorLookupFails_NamesAreNullButDoesNotThrow()
        {
            var filter = new AppointmentFilterDto { PageNumber = 1, PageSize = 20 };
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, DoctorId = 5, PatientId = 10, AppointmentProcedures = new List<AppointmentProcedure>() }
            };
            var dtos = new List<AppointmentListResponseDto> { new AppointmentListResponseDto { Id = 1, DoctorId = 5, PatientId = 10 } };

            appointmentRepositoryMock.Setup(r => r.GetAllAsync(filter)).ReturnsAsync((appointments, 1));
            mapperMock.Setup(m => m.Map<List<AppointmentListResponseDto>>(appointments)).Returns(dtos);
            queryServiceClientMock.Setup(c => c.GetDoctorAsync(5)).ReturnsAsync((DoctorResponseDto?)null);
            queryServiceClientMock.Setup(c => c.GetPatientAsync(10)).ReturnsAsync((PatientResponseDto?)null);

            var result = await appointmentService.GetAllAsync(filter);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Items[0].DoctorName, Is.Null);
            Assert.That(result.Data.Items[0].PatientName, Is.Null);
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
            queryServiceClientMock.Setup(h => h.GetPatientAsync(patientId)).ReturnsAsync(
                new PatientResponseDto { Id = patientId, Name = "Marko", LastName = "Petrovic", Email = "marko@test.com" });
            queryServiceClientMock.Setup(x => x.GetDoctorAsync(doctorId)).ReturnsAsync(
                new DoctorResponseDto { Id = doctorId, FirstName = "Ana", LastName = "Kovac" });
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
            int doctorId = 1;
            int patientId = 1;
            var request = new AppointmentUpdateRequestDto { Id = appointmentId, DoctorId = doctorId, PatientId = patientId };
            var appointment = new Appointment { Id = appointmentId };
            var appointmentDto = new AppointmentUpdateResponseDto { Id = appointmentId };

            appointmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(appointment);
            queryServiceClientMock.Setup(h => h.GetPatientAsync(patientId)).ReturnsAsync(
                new PatientResponseDto { Id = patientId, Name = "Marko", LastName = "Petrovic", Email = "marko@test.com" });
            queryServiceClientMock.Setup(x => x.GetDoctorAsync(doctorId)).ReturnsAsync(
                new DoctorResponseDto { Id = doctorId, FirstName = "Ana", LastName = "Kovac" });
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
        public async Task GetFreeSlotsAsync_PastDate_ReturnsInvalidDate()
        {
            var result = await appointmentService.GetFreeSlotsAsync(1, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)));

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DATE"));
        }

        [Test]
        public async Task GetFreeSlotsAsync_DoctorHasNoSchedule_ReturnsDoctorNotAvailable()
        {
            var doctorId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            queryServiceClientMock
                .Setup(c => c.GetDoctorScheduleAsync(doctorId, date.DayOfWeek))
                .ReturnsAsync((DoctorScheduleResponseDto?)null);

            var result = await appointmentService.GetFreeSlotsAsync(doctorId, date);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("DOCTOR_NOT_AVAILABLE"));
        }

        [Test]
        public async Task GetFreeSlotsAsync_NoAppointments_ReturnsAllSlots()
        {
            var doctorId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            queryServiceClientMock
                .Setup(c => c.GetDoctorScheduleAsync(doctorId, date.DayOfWeek))
                .ReturnsAsync(new DoctorScheduleResponseDto { StartHour = 8, EndHour = 10 });

            appointmentRepositoryMock
                .Setup(r => r.GetByDoctorIdAndDateAsync(doctorId, date))
                .ReturnsAsync(new List<Appointment>());

            var result = await appointmentService.GetFreeSlotsAsync(doctorId, date);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Has.Count.EqualTo(4)); // 8:00, 8:30, 9:00, 9:30 with 30 min slots
        }

        [Test]
        public async Task GetFreeSlotsAsync_AppointmentBooked_ExcludesBookedSlot()
        {
            var doctorId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            queryServiceClientMock
                .Setup(c => c.GetDoctorScheduleAsync(doctorId, date.DayOfWeek))
                .ReturnsAsync(new DoctorScheduleResponseDto { StartHour = 8, EndHour = 10 });

            var bookedAppointment = new Appointment
            {
                DateTime = date.ToDateTime(new TimeOnly(8, 0)),
                Duration = TimeSpan.FromMinutes(30)
            };

            appointmentRepositoryMock
                .Setup(r => r.GetByDoctorIdAndDateAsync(doctorId, date))
                .ReturnsAsync(new List<Appointment> { bookedAppointment });

            var result = await appointmentService.GetFreeSlotsAsync(doctorId, date);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Has.Count.EqualTo(3)); // 8:30, 9:00, 9:30
            Assert.That(result.Data!.Any(s => s.Start == new TimeOnly(8, 0)), Is.False);
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

        [Test]
        public async Task GetPopularDoctorIdsAsync_ValidCount_ReturnsSuccess()
        {
            var ids = new List<int> { 3, 1, 2 };
            appointmentRepositoryMock.Setup(r => r.GetTopDoctorIdsByAppointmentCountAsync(5)).ReturnsAsync(ids);

            var result = await appointmentService.GetPopularDoctorIdsAsync(5);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(ids));
        }

        [Test]
        public async Task GetPopularDoctorIdsAsync_CountIsZeroOrNegative_ReturnsFailure()
        {
            var result = await appointmentService.GetPopularDoctorIdsAsync(0);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_COUNT"));
            appointmentRepositoryMock.Verify(r => r.GetTopDoctorIdsByAppointmentCountAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetPopularPatientIdsAsync_ValidCount_ReturnsSuccess()
        {
            var ids = new List<int> { 11, 1, 2 };
            appointmentRepositoryMock.Setup(r => r.GetTopPatientIdsByAppointmentCountAsync(5)).ReturnsAsync(ids);

            var result = await appointmentService.GetPopularPatientIdsAsync(5);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(ids));
        }

        [Test]
        public async Task GetPopularPatientIdsAsync_CountIsZeroOrNegative_ReturnsFailure()
        {
            var result = await appointmentService.GetPopularPatientIdsAsync(-1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_COUNT"));
            appointmentRepositoryMock.Verify(r => r.GetTopPatientIdsByAppointmentCountAsync(It.IsAny<int>()), Times.Never);
        }
    }
}