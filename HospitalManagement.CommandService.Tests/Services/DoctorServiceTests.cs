using AutoMapper;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Implementations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;

namespace HospitalManagement.CommandService.Tests.Services
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
            cachePolicy = Policy.NoOpAsync();

            doctorService = new DoctorService(
                doctorRepositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object,
                cacheMock.Object,
                cachePolicy);
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess()
        {
            var request = new DoctorCreateRequestDto { FirstName = "Test" };
            var doctor = new Doctor { Id = 1, FirstName = "Test" };
            var doctorDto = new DoctorResponseDto { Id = 1 };

            mapperMock.Setup(m => m.Map<Doctor>(request)).Returns(doctor);
            doctorRepositoryMock.Setup(r => r.CreateAsync(doctor)).ReturnsAsync(doctor);
            mapperMock.Setup(m => m.Map<DoctorResponseDto>(doctor)).Returns(doctorDto);

            var result = await doctorService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorDto));
        }

        [Test]
        public async Task UpdateAsync_DoctorExists_ReturnsSuccessAndInvalidatesCache()
        {
            var request = new DoctorUpdateRequestDto { Id = 1, FirstName = "Test" };
            var doctor = new Doctor { Id = 1 };
            var doctorDto = new DoctorResponseDto { Id = 1 };

            doctorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(doctor);
            doctorRepositoryMock.Setup(r => r.UpdateAsync(doctor)).ReturnsAsync(doctor);
            mapperMock.Setup(m => m.Map<DoctorResponseDto>(doctor)).Returns(doctorDto);
            cacheMock.Setup(c => c.RemoveAsync("doctor:1", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await doctorService.UpdateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(doctorDto));
            cacheMock.Verify(c => c.RemoveAsync("doctor:1", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_DoctorNotFound_ReturnsFailure()
        {
            var request = new DoctorUpdateRequestDto { Id = 1 };

            doctorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Doctor?)null);

            var result = await doctorService.UpdateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.NotFound));
        }

        [Test]
        public async Task Delete_DoctorExists_ReturnsSuccessAndInvalidatesCache()
        {
            var doctor = new Doctor { Id = 1 };

            doctorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(doctor);
            cacheMock.Setup(c => c.RemoveAsync("doctor:1", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await doctorService.Delete(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor has been deleted!"));
            cacheMock.Verify(c => c.RemoveAsync("doctor:1", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Delete_DoctorNotFound_ReturnsFailure()
        {
            doctorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Doctor?)null);

            var result = await doctorService.Delete(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.NotFound));
        }
    }
}
