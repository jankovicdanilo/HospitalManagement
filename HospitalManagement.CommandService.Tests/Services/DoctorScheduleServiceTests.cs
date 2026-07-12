using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.CommandService.Tests.Services
{
    [TestFixture]
    internal class DoctorScheduleServiceTests
    {
        private Mock<IDoctorScheduleRepository> doctorScheduleRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<DoctorScheduleService>> loggerMock;
        private DoctorScheduleService doctorScheduleService;

        [SetUp]
        public void SetUp()
        {
            doctorScheduleRepositoryMock = new Mock<IDoctorScheduleRepository>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<DoctorScheduleService>>();

            doctorScheduleService = new DoctorScheduleService(
                doctorScheduleRepositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object);
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess()
        {
            var request = new DoctorScheduleCreateRequestDto { DoctorId = 1, DayOfWeek = DayOfWeek.Monday };
            var doctorSchedule = new DoctorSchedule { Id = 1, DoctorId = 1 };
            var doctorScheduleDto = new DoctorScheduleCreateResponseDto { Id = 1 };

            doctorScheduleRepositoryMock.Setup(r => r.DoctorExists(1)).ReturnsAsync(true);
            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(request.DoctorId, request.DayOfWeek)).ReturnsAsync((DoctorSchedule?)null);
            mapperMock.Setup(m => m.Map<DoctorSchedule>(request)).Returns(doctorSchedule);
            doctorScheduleRepositoryMock.Setup(r => r.CreateAsync(doctorSchedule)).ReturnsAsync(doctorSchedule);
            mapperMock.Setup(m => m.Map<DoctorScheduleCreateResponseDto>(doctorSchedule)).Returns(doctorScheduleDto);

            var result = await doctorScheduleService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorScheduleDto));
        }

        [Test]
        public async Task CreateAsync_DoctorNotFound_ReturnsFailure()
        {
            var request = new DoctorScheduleCreateRequestDto { DoctorId = 1 };

            doctorScheduleRepositoryMock.Setup(r => r.DoctorExists(1)).ReturnsAsync(false);

            var result = await doctorScheduleService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DOCTOR_ID"));
        }

        [Test]
        public async Task CreateAsync_DuplicateSchedule_ReturnsFailure()
        {
            var request = new DoctorScheduleCreateRequestDto { DoctorId = 1, DayOfWeek = DayOfWeek.Monday };
            var doctorSchedule = new DoctorSchedule { Id = 1, DoctorId = 1 };

            doctorScheduleRepositoryMock.Setup(r => r.DoctorExists(1)).ReturnsAsync(true);
            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(request.DoctorId, request.DayOfWeek)).ReturnsAsync(doctorSchedule);

            var result = await doctorScheduleService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("DUPLICATE_SCHEDULE"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsSuccess()
        {
            int doctorId = 1;
            DayOfWeek dayOfWeek = DayOfWeek.Monday;
            var request = new DoctorScheduleUpdateRequestDto { Id = 1, DayOfWeek = dayOfWeek };
            var doctorSchedule = new DoctorSchedule { Id = 1, DoctorId = doctorId, DayOfWeek = dayOfWeek };
            var doctorScheduleDto = new DoctorScheduleUpdateResponseDto { Id = 1, DoctorId = doctorId, DayOfWeek = dayOfWeek };

            doctorScheduleRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(doctorSchedule);
            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(doctorSchedule.DoctorId, request.DayOfWeek)).ReturnsAsync((DoctorSchedule?)null);
            doctorScheduleRepositoryMock.Setup(r => r.UpdateAsync(doctorSchedule)).ReturnsAsync(doctorSchedule);
            mapperMock.Setup(m => m.Map<DoctorScheduleUpdateResponseDto>(doctorSchedule)).Returns(doctorScheduleDto);

            var result = await doctorScheduleService.UpdateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorScheduleDto));
        }

        [Test]
        public async Task UpdateAsync_ScheduleNotFound_ReturnsFailure()
        {
            var request = new DoctorScheduleUpdateRequestDto { Id = 1, DayOfWeek = DayOfWeek.Monday };

            doctorScheduleRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((DoctorSchedule?)null);

            var result = await doctorScheduleService.UpdateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task UpdateAsync_DuplicateSchedule_ReturnsFailure()
        {
            DayOfWeek dayOfWeek = DayOfWeek.Monday;
            var request = new DoctorScheduleUpdateRequestDto { Id = 1, DayOfWeek = dayOfWeek };
            var doctorSchedule = new DoctorSchedule { Id = 1, DoctorId = 1, DayOfWeek = dayOfWeek };
            var duplicateSchedule = new DoctorSchedule { Id = 2, DoctorId = 1, DayOfWeek = dayOfWeek };

            doctorScheduleRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(doctorSchedule);
            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(doctorSchedule.DoctorId, request.DayOfWeek)).ReturnsAsync(duplicateSchedule);

            var result = await doctorScheduleService.UpdateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("DUPLICATE_SCHEDULE"));
        }

        [Test]
        public async Task Delete_ScheduleExists_ReturnsSuccess()
        {
            var doctorSchedule = new DoctorSchedule { Id = 1 };

            doctorScheduleRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync(doctorSchedule);

            var result = await doctorScheduleService.Delete(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor schedule with id 1 deleted"));
        }

        [Test]
        public async Task Delete_ScheduleNotFound_ReturnsFailure()
        {
            doctorScheduleRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync((DoctorSchedule?)null);

            var result = await doctorScheduleService.Delete(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }
    }
}
