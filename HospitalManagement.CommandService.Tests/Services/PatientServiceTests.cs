using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.CommandService.Tests.Services
{
    [TestFixture]
    internal class PatientServiceTests
    {
        private Mock<IPatientRepository> patientRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<PatientService>> loggerMock;
        private PatientService patientService;

        [SetUp]
        public void SetUp()
        {
            patientRepositoryMock = new Mock<IPatientRepository>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<PatientService>>();

            patientService = new PatientService(
                patientRepositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object);
        }

        [Test]
        public async Task CreateAsync_EmailNotTaken_ReturnsSuccess()
        {
            var request = new PatientCreateRequestDto { Email = "test@test.com" };
            var patient = new Patient { Id = 1, Email = "test@test.com" };
            var patientDto = new PatientCreateResponseDto { Id = 1 };

            patientRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((Patient?)null);
            mapperMock.Setup(m => m.Map<Patient>(request)).Returns(patient);
            patientRepositoryMock.Setup(r => r.CreateAsync(patient)).ReturnsAsync(patient);
            mapperMock.Setup(m => m.Map<PatientCreateResponseDto>(patient)).Returns(patientDto);

            var result = await patientService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(patientDto));
        }

        [Test]
        public async Task CreateAsync_EmailAlreadyExists_ReturnsFailure()
        {
            var request = new PatientCreateRequestDto { Email = "test@test.com" };
            var existingPatient = new Patient { Email = "test@test.com" };

            patientRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingPatient);

            var result = await patientService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_EMAIL"));
        }

        [Test]
        public async Task UpdateAsync_Success_ReturnsSuccess()
        {
            var request = new PatientUpdateRequestDto { Id = 1, Email = "test@test.com" };
            var patient = new Patient { Id = 1, Email = "test@test.com" };
            var patientDto = new PatientUpdateResponseDto { Id = 1 };

            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            patientRepositoryMock.Setup(r => r.EmailExists(request.Email)).ReturnsAsync(false);
            patientRepositoryMock.Setup(r => r.UpdateAsync(patient)).ReturnsAsync(patient);
            mapperMock.Setup(m => m.Map<PatientUpdateResponseDto>(patient)).Returns(patientDto);

            var result = await patientService.UpdateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(patientDto));
        }

        [Test]
        public async Task UpdateAsync_PatientNotFound_ReturnsFailure()
        {
            var request = new PatientUpdateRequestDto { Id = 1 };

            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Patient?)null);

            var result = await patientService.UpdateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task UpdateAsync_EmailTakenByAnotherPatient_ReturnsFailure()
        {
            var request = new PatientUpdateRequestDto { Id = 1, Email = "taken@test.com" };
            var patient = new Patient { Id = 1, Email = "original@test.com" };

            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            patientRepositoryMock.Setup(r => r.EmailExists("taken@test.com")).ReturnsAsync(true);

            var result = await patientService.UpdateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_EMAIL"));
        }

        [Test]
        public async Task UpdateAsync_EmailUnchanged_ReturnsSuccess()
        {
            var request = new PatientUpdateRequestDto { Id = 1, Email = "same@test.com" };
            var patient = new Patient { Id = 1, Email = "same@test.com" };
            var patientDto = new PatientUpdateResponseDto { Id = 1 };

            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            patientRepositoryMock.Setup(r => r.EmailExists("same@test.com")).ReturnsAsync(true);
            patientRepositoryMock.Setup(r => r.UpdateAsync(patient)).ReturnsAsync(patient);
            mapperMock.Setup(m => m.Map<PatientUpdateResponseDto>(patient)).Returns(patientDto);

            var result = await patientService.UpdateAsync(request);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task Delete_PatientExists_ReturnsSuccess()
        {
            var patient = new Patient { Id = 1 };

            patientRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync(patient);

            var result = await patientService.Delete(1);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task Delete_PatientNotFound_ReturnsFailure()
        {
            patientRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync((Patient?)null);

            var result = await patientService.Delete(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }
    }
}
