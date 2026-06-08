using Moq;
using HospitalManagement.Repositories.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using HospitalManagement.Services.Implementations;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;

namespace HospitalManagement.Tests.Services
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

            patientService = new PatientService
                (
                    patientRepositoryMock.Object,
                    mapperMock.Object,
                    loggerMock.Object
                );
        }

        [Test]
        public async Task Delete_PatientExists_ReturnsSuccess()
        {
            var patient = new Patient { Id = 1};
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

        [Test]
        public async Task GetById_PatientExists_ReturnsSuccess()
        {
            var patient = new Patient { Id = 1, Name = "John" };
            var patientDto = new PatientGetByIdDto { Id = 1, Name = "John" };

            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            mapperMock.Setup(m => m.Map<PatientGetByIdDto>(patient)).Returns(patientDto);

            var result = await patientService.GetByIdAsync(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(patientDto));
        }

        [Test]
        public async Task GetById_PatientNotFound_ReturnsFailure()
        {
            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Patient?)null);

            var result = await patientService.GetByIdAsync(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task CreateAsync_EmailNotTaken_ReturnsSuccess()
        {
            var request = new PatientCreateRequestDto { Email = "test@.com" };
            var patient = new Patient { Id = 1, Email = "test@.com" };
            var patientDto = new PatientCreateResponseDto { Id = 1 };

            patientRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((Patient?)null);
            mapperMock.Setup(r => r.Map<Patient>(request)).Returns(patient);
            patientRepositoryMock.Setup(r => r.CreateAsync(patient)).ReturnsAsync(patient);
            mapperMock.Setup(r => r.Map<PatientCreateResponseDto>(patient)).Returns(patientDto);

            var result = await patientService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(patientDto));
        }

        [Test]
        public async Task CreateAsync_EmailAlreadyExists_ReturnsFailure()
        {
            var request = new PatientCreateRequestDto { Email = "test@.com" };
            var existingPatient = new Patient { Email = "test@.com" };

            patientRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingPatient);

            var result = await patientService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_EMAIL"));
        }

        [Test]
        public async Task UpdateAsync_Success_ReturnsSuccess()
        {
            var request = new PatientUpdateRequestDto { Id = 1, Email = "test@.com" };
            var patient = new Patient { Id = 1, Email = "test@.com" };
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
        public async Task GetMedicalHistoryAsync_PatientExists_ReturnsSuccess()
        {
            var patient = new Patient { Id = 1 };
            var dto = new PatientMedicalHistoryDto { };

            patientRepositoryMock.Setup(r => r.GetMedicalHistoryAsync(1)).ReturnsAsync(patient);
            mapperMock.Setup(m => m.Map<PatientMedicalHistoryDto>(patient)).Returns(dto);

            var result = await patientService.GetMedicalHistoryAsync(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(dto));
        }

        [Test]
        public async Task GetMedicalHistoryAsync_PatientNotFound_ReturnsFailure()
        {
            patientRepositoryMock.Setup(r => r.GetMedicalHistoryAsync(1)).ReturnsAsync((Patient?)null);

            var result = await patientService.GetMedicalHistoryAsync(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }
    }
}
