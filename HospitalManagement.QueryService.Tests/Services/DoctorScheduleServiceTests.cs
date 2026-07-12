using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.QueryService.Tests.Services
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
        public async Task GetByIdAsync_ScheduleExists_ReturnsSuccess()
        {
            int doctorScheduleId = 1;
            DayOfWeek dayOfWeek = DayOfWeek.Monday;
            var doctorSchedule = new DoctorSchedule { Id = doctorScheduleId, DayOfWeek = dayOfWeek };
            var doctorScheduleDto = new DoctorScheduleResponseDto { Id = doctorScheduleId, DayOfWeek = dayOfWeek };

            doctorScheduleRepositoryMock.Setup(r => r.GetByIdAsync(doctorScheduleId)).ReturnsAsync(doctorSchedule);
            mapperMock.Setup(m => m.Map<DoctorScheduleResponseDto>(doctorSchedule)).Returns(doctorScheduleDto);

            var result = await doctorScheduleService.GetByIdAsync(doctorScheduleId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorScheduleDto));
        }

        [Test]
        public async Task GetByIdAsync_ScheduleNotFound_ReturnsFailure()
        {
            int doctorScheduleId = 1;

            doctorScheduleRepositoryMock.Setup(r => r.GetByIdAsync(doctorScheduleId)).ReturnsAsync((DoctorSchedule?)null);

            var result = await doctorScheduleService.GetByIdAsync(doctorScheduleId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task GetAllByDoctorIdAsync_DoctorExists_ReturnsSuccess()
        {
            int doctorId = 1;
            var doctorSchedules = new List<DoctorSchedule>();
            var doctorScheduleResponsesDto = new List<DoctorScheduleResponseDto>();

            doctorScheduleRepositoryMock.Setup(r => r.DoctorExists(doctorId)).ReturnsAsync(true);
            doctorScheduleRepositoryMock.Setup(r => r.GetAllByDoctorIdAsync(doctorId)).ReturnsAsync(doctorSchedules);
            mapperMock.Setup(m => m.Map<List<DoctorScheduleResponseDto>>(doctorSchedules)).Returns(doctorScheduleResponsesDto);

            var result = await doctorScheduleService.GetAllByDoctorIdAsync(doctorId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorScheduleResponsesDto));
        }

        [Test]
        public async Task GetAllByDoctorIdAsync_DoctorNotFound_ReturnsFailure()
        {
            int doctorId = 1;

            doctorScheduleRepositoryMock.Setup(r => r.DoctorExists(doctorId)).ReturnsAsync(false);

            var result = await doctorScheduleService.GetAllByDoctorIdAsync(doctorId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DOCTOR_ID"));
        }

        [Test]
        public async Task GetByDoctorIdAndDayAsync_ReturnsSuccess()
        {
            int doctorId = 1;
            DayOfWeek dayOfWeek = DayOfWeek.Monday;
            var doctorSchedule = new DoctorSchedule { DoctorId = doctorId, DayOfWeek = dayOfWeek };
            var doctorScheduleDto = new DoctorScheduleResponseDto { DoctorId = doctorId, DayOfWeek = dayOfWeek };

            doctorScheduleRepositoryMock.Setup(r => r.DoctorExists(doctorId)).ReturnsAsync(true);
            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek)).ReturnsAsync(doctorSchedule);
            mapperMock.Setup(m => m.Map<DoctorScheduleResponseDto>(doctorSchedule)).Returns(doctorScheduleDto);

            var result = await doctorScheduleService.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorScheduleDto));
        }

        [Test]
        public async Task GetByDoctorIdAndDayAsync_DoctorNotFound_ReturnsFailure()
        {
            int doctorId = 1;
            DayOfWeek dayOfWeek = DayOfWeek.Monday;

            doctorScheduleRepositoryMock.Setup(r => r.DoctorExists(doctorId)).ReturnsAsync(false);

            var result = await doctorScheduleService.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DOCTOR_ID"));
        }

        [Test]
        public async Task GetByDoctorIdAndDayAsync_ScheduleNotFound_ReturnsFailure()
        {
            int doctorId = 1;
            DayOfWeek dayOfWeek = DayOfWeek.Monday;

            doctorScheduleRepositoryMock.Setup(r => r.DoctorExists(doctorId)).ReturnsAsync(true);
            doctorScheduleRepositoryMock.Setup(r => r.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek)).ReturnsAsync((DoctorSchedule?)null);

            var result = await doctorScheduleService.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("DOCTOR_NOT_AVAILABLE"));
        }
    }
}
