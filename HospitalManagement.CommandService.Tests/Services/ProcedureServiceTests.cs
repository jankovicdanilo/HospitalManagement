using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.CommandService.Tests.Services
{
    [TestFixture]
    internal class ProcedureServiceTests
    {
        private Mock<IProcedureRepository> procedureRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<ProcedureService>> loggerMock;
        private ProcedureService procedureService;

        [SetUp]
        public void SetUp()
        {
            procedureRepositoryMock = new Mock<IProcedureRepository>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<ProcedureService>>();

            procedureService = new ProcedureService(
                procedureRepositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object);
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess()
        {
            var request = new ProcedureCreateRequestDto { Name = "Test" };
            var procedure = new Procedure { Id = 1, Name = "Test" };
            var procedureDto = new ProcedureCreateResponseDto { Id = 1 };

            mapperMock.Setup(m => m.Map<Procedure>(request)).Returns(procedure);
            procedureRepositoryMock.Setup(r => r.CreateAsync(procedure)).ReturnsAsync(procedure);
            mapperMock.Setup(m => m.Map<ProcedureCreateResponseDto>(procedure)).Returns(procedureDto);

            var result = await procedureService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(procedureDto));
        }

        [Test]
        public async Task UpdateAsync_ProcedureExists_ReturnsSuccess()
        {
            var request = new ProcedureUpdateRequestDto { Name = "Test", Price = 120 };
            var procedure = new Procedure { Id = 1, Name = "Test", Price = 50 };
            var procedureDto = new ProcedureUpdateResponseDto { Id = 1 };

            mapperMock.Setup(m => m.Map<Procedure>(request)).Returns(procedure);
            procedureRepositoryMock.Setup(r => r.UpdateAsync(1, procedure)).ReturnsAsync(procedure);
            mapperMock.Setup(m => m.Map<ProcedureUpdateResponseDto>(procedure)).Returns(procedureDto);

            var result = await procedureService.UpdateAsync(1, request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(procedureDto));
        }

        [Test]
        public async Task UpdateAsync_ProcedureNotFound_ReturnsFailure()
        {
            var request = new ProcedureUpdateRequestDto { Name = "Test", Price = 120 };
            var procedure = new Procedure { Id = 1, Name = "Test", Price = 50 };

            mapperMock.Setup(m => m.Map<Procedure>(request)).Returns(procedure);
            procedureRepositoryMock.Setup(r => r.UpdateAsync(1, procedure)).ReturnsAsync((Procedure?)null);

            var result = await procedureService.UpdateAsync(1, request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task DeleteAsync_ProcedureExists_ReturnsSuccess()
        {
            var procedure = new Procedure { Id = 1, Name = "Test" };

            procedureRepositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(procedure);

            var result = await procedureService.DeleteAsync(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Procedure deleted"));
        }

        [Test]
        public async Task DeleteAsync_ProcedureNotFound_ReturnsFailure()
        {
            procedureRepositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync((Procedure?)null);

            var result = await procedureService.DeleteAsync(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }
    }
}
