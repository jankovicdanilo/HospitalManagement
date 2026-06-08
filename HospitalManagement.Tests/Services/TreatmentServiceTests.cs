using AutoMapper;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Treatment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Implementations;
using HospitalManagement.Services.Validations;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.Tests.Services
{
    [TestFixture]
    internal class TreatmentServiceTests
    {
        private Mock<ITreatmentRepository> treatmentRepositoryMock;
        private Mock<ITreatmentValidation> treatmentValidationMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<TreatmentService>> loggerMock;
        private TreatmentService treatmentService;

        [SetUp]
        public void SetUp()
        {
            treatmentRepositoryMock = new Mock<ITreatmentRepository>();
            treatmentValidationMock = new Mock<ITreatmentValidation>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<TreatmentService>>();

            treatmentService = new TreatmentService
                (
                    treatmentRepositoryMock.Object, 
                    mapperMock.Object,
                    treatmentValidationMock.Object,
                    loggerMock.Object
                );
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            var request = new TreatmentCreateRequestDto { AppointmentId = appointmentId };
            var treatment = new Treatment { AppointmentId= appointmentId };
            var treatmentDto = new TreatmentCreateResponseDto { AppointmentId = appointmentId };

            treatmentValidationMock.Setup(v => v.ValidateAll(request))
                .ReturnsAsync(Result.Ok("Validation ok"));
            mapperMock.Setup(m => m.Map<Treatment>(request)).Returns(treatment);
            treatmentRepositoryMock.Setup(r => r.CreateAsync(treatment)).ReturnsAsync(treatment);
            mapperMock.Setup(m => m.Map<TreatmentCreateResponseDto>(treatment)).Returns(treatmentDto);

            var result = await treatmentService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(treatmentDto));
        }

        [Test]
        public async Task CreateAsync_ValidationFails_ReturnsFailure()
        {
            int appointmentId = 1;
            var request = new TreatmentCreateRequestDto { AppointmentId = appointmentId };

            treatmentValidationMock.Setup(v => v.ValidateAll(request))
                .ReturnsAsync(Result.Fail(
                    $"Appointment with id {request.AppointmentId} not found",
                    "APPOINTMENT_NOT_FOUND"));

            var result = await treatmentService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("APPOINTMENT_NOT_FOUND"));
        }
    }
}
