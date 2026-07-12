using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.QueryService.Tests.Services
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
        public async Task GetByIdAsync_ProcedureExists_ReturnsSuccess()
        {
            var procedure = new Procedure { Id = 1, Name = "Test" };
            var procedureDto = new ProcedureResponseDto { Id = 1, Name = "Test" };

            procedureRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(procedure);
            mapperMock.Setup(m => m.Map<ProcedureResponseDto>(procedure)).Returns(procedureDto);

            var result = await procedureService.GetByIdAsync(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(procedureDto));
        }

        [Test]
        public async Task GetByIdAsync_ProcedureNotFound_ReturnsFailure()
        {
            procedureRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Procedure?)null);

            var result = await procedureService.GetByIdAsync(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task GetAllAsync_ReturnsSuccess()
        {
            var procedures = new List<Procedure> { new Procedure { Id = 1, Name = "Test" } };
            var procedureDtos = new List<ProcedureListDto> { new ProcedureListDto { Id = 1, Name = "Test" } };

            procedureRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(procedures);
            mapperMock.Setup(m => m.Map<List<ProcedureListDto>>(procedures)).Returns(procedureDtos);

            var result = await procedureService.GetAllAsync();

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(procedureDtos));
        }

        [Test]
        public async Task GetAllAsync_NoProcedures_ReturnsSuccessWithEmptyList()
        {
            var procedures = new List<Procedure>();
            var procedureDtos = new List<ProcedureListDto>();

            procedureRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(procedures);
            mapperMock.Setup(m => m.Map<List<ProcedureListDto>>(procedures)).Returns(procedureDtos);

            var result = await procedureService.GetAllAsync();

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Empty);
        }
    }
}
