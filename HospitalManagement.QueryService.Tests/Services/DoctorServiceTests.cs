using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Implementations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using System.Text;
using System.Text.Json;

namespace HospitalManagement.QueryService.Tests.Services
{
    [TestFixture]
    internal class DoctorServiceTests
    {
        private Mock<IDoctorRepository> doctorRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<DoctorService>> loggerMock;
        private Mock<IDistributedCache> cacheMock;
        private IAsyncPolicy cachePolicy;
        private DoctorService doctorService;

        [SetUp]
        public void SetUp()
        {
            doctorRepositoryMock = new Mock<IDoctorRepository>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<DoctorService>>();
            cacheMock = new Mock<IDistributedCache>();
            cachePolicy = Policy.NoOpAsync(); // real no-op policy, not mocked — ExecuteAsync<T> is an extension method

            doctorService = new DoctorService(
                doctorRepositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object,
                cacheMock.Object,
                cachePolicy);
        }

        [Test]
        public async Task GetByIdAsync_CacheMiss_ReturnsFromRepositoryAndCaches()
        {
            var doctor = new Doctor { Id = 1 };
            var doctorDto = new DoctorResponseDto { Id = 1 };

            cacheMock.Setup(c => c.GetAsync("doctor:1", It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);
            doctorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(doctor);
            mapperMock.Setup(m => m.Map<DoctorResponseDto>(doctor)).Returns(doctorDto);
            cacheMock.Setup(c => c.SetAsync("doctor:1", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await doctorService.GetByIdAsync(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorDto));
            cacheMock.Verify(c => c.SetAsync("doctor:1", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_CacheHit_ReturnsFromCacheWithoutHittingRepository()
        {
            var doctorDto = new DoctorResponseDto { Id = 1 };
            var cachedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(doctorDto));

            cacheMock.Setup(c => c.GetAsync("doctor:1", It.IsAny<CancellationToken>())).ReturnsAsync(cachedBytes);

            var result = await doctorService.GetByIdAsync(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Id, Is.EqualTo(doctorDto.Id));
            doctorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetByIdAsync_DoctorNotFound_ReturnsFailure()
        {
            cacheMock.Setup(c => c.GetAsync("doctor:1", It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);
            doctorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Doctor?)null);

            var result = await doctorService.GetByIdAsync(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task GetAllAsync_ReturnsSuccess()
        {
            var doctors = new List<Doctor> { new Doctor { Id = 1 }, new Doctor { Id = 2 } };
            var doctorDtos = new List<DoctorResponseDto> { new DoctorResponseDto { Id = 1 }, new DoctorResponseDto { Id = 2 } };

            doctorRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(doctors);
            mapperMock.Setup(m => m.Map<List<DoctorResponseDto>>(doctors)).Returns(doctorDtos);

            var result = await doctorService.GetAllAsync();

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorDtos));
        }

        [Test]
        public async Task GetAllAsync_NoDoctors_ReturnsSuccessWithEmptyList()
        {
            var doctors = new List<Doctor>();
            var doctorDtos = new List<DoctorResponseDto>();

            doctorRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(doctors);
            mapperMock.Setup(m => m.Map<List<DoctorResponseDto>>(doctors)).Returns(doctorDtos);

            var result = await doctorService.GetAllAsync();

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Empty);
        }
    }
}
