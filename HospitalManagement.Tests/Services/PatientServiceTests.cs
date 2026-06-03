using Moq;
using HospitalManagement.Repositories.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using HospitalManagement.Services.Implementations;

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

    }
}
