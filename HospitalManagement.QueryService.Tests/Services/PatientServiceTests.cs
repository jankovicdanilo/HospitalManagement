using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Clients.Interfaces;
using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.QueryService.Tests.Services
{
    [TestFixture]
    internal class PatientServiceTests
    {
        private Mock<IPatientRepository> patientRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<PatientService>> loggerMock;
        private Mock<IAppointmentServiceClient> appointmentServiceClientMock;
        private PatientService patientService;

        [SetUp]
        public void SetUp()
        {
            patientRepositoryMock = new Mock<IPatientRepository>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<PatientService>>();
            appointmentServiceClientMock = new Mock<IAppointmentServiceClient>();

            patientService = new PatientService(
                patientRepositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object,
                appointmentServiceClientMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsSuccess()
        {
            var patients = new List<Patient> { new Patient { Id = 1, Name = "John" } };
            var patientDtos = new List<PatientListDto> { new PatientListDto { Id = 1, Name = "John", LastName = "Doe", Email = "j@d.com" } };

            patientRepositoryMock.Setup(r => r.GetAllAsync(1, 20)).ReturnsAsync((patients, patients.Count));
            mapperMock.Setup(m => m.Map<List<PatientListDto>>(patients)).Returns(patientDtos);

            var result = await patientService.GetAllAsync(1, 20);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Items, Is.EqualTo(patientDtos));
            Assert.That(result.Data.TotalCount, Is.EqualTo(patients.Count));
            Assert.That(result.Data.PageNumber, Is.EqualTo(1));
            Assert.That(result.Data.PageSize, Is.EqualTo(20));
        }

        [Test]
        public async Task GetAllAsync_NoPatients_ReturnsSuccessWithEmptyList()
        {
            var patients = new List<Patient>();
            var patientDtos = new List<PatientListDto>();

            patientRepositoryMock.Setup(r => r.GetAllAsync(1, 20)).ReturnsAsync((patients, 0));
            mapperMock.Setup(m => m.Map<List<PatientListDto>>(patients)).Returns(patientDtos);

            var result = await patientService.GetAllAsync(1, 20);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Items, Is.Empty);
            Assert.That(result.Data.TotalCount, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllAsync_SecondPage_PassesCorrectPageNumberToRepository()
        {
            var patients = new List<Patient> { new Patient { Id = 21, Name = "Jane" } };
            var patientDtos = new List<PatientListDto> { new PatientListDto { Id = 21, Name = "Jane", LastName = "Doe", Email = "jane@d.com" } };

            patientRepositoryMock.Setup(r => r.GetAllAsync(2, 20)).ReturnsAsync((patients, 21));
            mapperMock.Setup(m => m.Map<List<PatientListDto>>(patients)).Returns(patientDtos);

            var result = await patientService.GetAllAsync(2, 20);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.PageNumber, Is.EqualTo(2));
            patientRepositoryMock.Verify(r => r.GetAllAsync(2, 20), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_PatientExists_ReturnsSuccess()
        {
            var patient = new Patient { Id = 1, Name = "John" };
            var patientDto = new PatientGetByIdDto { Id = 1, Name = "John", LastName = "Doe", Email = "j@d.com" };

            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            mapperMock.Setup(m => m.Map<PatientGetByIdDto>(patient)).Returns(patientDto);

            var result = await patientService.GetByIdAsync(1);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(patientDto));
        }

        [Test]
        public async Task GetByIdAsync_PatientNotFound_ReturnsFailure()
        {
            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Patient?)null);

            var result = await patientService.GetByIdAsync(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task GetMedicalHistoryAsync_PatientAndHistoryExist_ReturnsSuccessWithPatientNameSet()
        {
            int patientId = 1;
            var patient = new Patient { Id = patientId, Name = "John", LastName = "Doe" };
            var history = new PatientMedicalHistoryDto { PatientId = patientId, PatientName = "placeholder" };

            patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
            appointmentServiceClientMock.Setup(c => c.GetPatientHistoryAsync(patientId)).ReturnsAsync(history);

            var result = await patientService.GetMedicalHistoryAsync(patientId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.PatientName, Is.EqualTo("John Doe"));
        }

        [Test]
        public async Task GetMedicalHistoryAsync_PatientNotFound_ReturnsFailure()
        {
            patientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Patient?)null);

            var result = await patientService.GetMedicalHistoryAsync(1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_PATIENT_ID"));
            appointmentServiceClientMock.Verify(c => c.GetPatientHistoryAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetMedicalHistoryAsync_HistoryUnavailable_ReturnsFailure()
        {
            int patientId = 1;
            var patient = new Patient { Id = patientId, Name = "John", LastName = "Doe" };

            patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
            appointmentServiceClientMock.Setup(c => c.GetPatientHistoryAsync(patientId)).ReturnsAsync((PatientMedicalHistoryDto?)null);

            var result = await patientService.GetMedicalHistoryAsync(patientId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("HISTORY_UNAVAILABLE"));
        }
    }
}
