using AutoMapper;
using Azure.Core;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Models.Enums;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Implementations;
using HospitalManagement.Services.Validations;
using HospitalManagement.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace HospitalManagement.Tests.Services
{
    [TestFixture]
    internal class AppointmentServiceTests
    {
        private Mock<IAppointmentRepository> appointmentRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<IAppointmentValidation> appointmentValidationMock;
        private Mock<ILogger<AppointmentService>> loggerMock;
        private Mock<IPatientRepository> patientRepositoryMock;
        private Mock<IDoctorRepository> doctorRepositoryMock;
        private Mock<IDoctorScheduleRepository> doctorScheduleRepositoryMock;
        private IOptions<AppointmentSettings> appointmentSettings;
        private AppointmentService appointmentService;

        [SetUp]
        public void SetUp()
        {
            appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            mapperMock = new Mock<IMapper>();
            appointmentValidationMock = new Mock<IAppointmentValidation>();
            loggerMock = new Mock<ILogger<AppointmentService>>();
            patientRepositoryMock = new Mock<IPatientRepository>();
            doctorRepositoryMock = new Mock<IDoctorRepository>();
            doctorScheduleRepositoryMock = new Mock<IDoctorScheduleRepository>();
            appointmentSettings = Options.Create(new AppointmentSettings { SlotSizeMinutes = 30 });

            appointmentService = new AppointmentService
                (
                    appointmentRepositoryMock.Object,
                    mapperMock.Object,
                    appointmentValidationMock.Object,
                    loggerMock.Object,
                    patientRepositoryMock.Object,
                    doctorRepositoryMock.Object,
                    doctorScheduleRepositoryMock.Object,
                    appointmentSettings
                );
        }

        [Test]
        public async Task GetByIdAsync_AppointmentExists_ReturnsSuccess()
        {
            int appointmentId = 1;
            var appointment = new Appointment { Id = appointmentId };
            var appointmentDto = new AppointmentResponseDto { Id = appointmentId };

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);
            mapperMock.Setup(m => m.Map<AppointmentResponseDto>(appointment)).Returns(appointmentDto);

            var result = await appointmentService.GetByIdAsync(appointmentId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(appointmentDto));
        }

        [Test]
        public async Task GetByIdAsync_AppointmentNotFound_ReturnsFailure()
        {
            int appointmentId = 1;

            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync((Appointment?)null);

            var result = await appointmentService.GetByIdAsync(appointmentId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ID"));
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess()
        {
            int appointmentId = 1;
            int doctorId = 1;
            int patientId = 1;
            var request = new AppointmentCreateRequestDto { PatientId = patientId, DoctorId = doctorId };
            var appointment = new Appointment {Id = appointmentId};
            var appointmentDto = new AppointmentCreateResponseDto {  Id = appointmentId };
            var doctor = new Doctor { Id = 1, FirstName = "John", LastName = "Doe" };
            var patient = new Patient { Id = 1, Email = "test@test.com" };

            appointmentValidationMock.Setup(v => v.ValidateAll(request)).ReturnsAsync(Result.Ok("Validation ok"));
            mapperMock.Setup(m => m.Map<Appointment>(request)).Returns(appointment);
            appointmentRepositoryMock.Setup(r => r.CreateAsync(appointment)).ReturnsAsync(appointment);
            patientRepositoryMock.Setup(p => p.GetByIdAsync(patientId)).ReturnsAsync(patient);
            doctorRepositoryMock.Setup(d => d.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
            mapperMock.Setup(m => m.Map<AppointmentCreateResponseDto>(appointment)).Returns(appointmentDto);

            var result = await appointmentService.CreateAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(appointmentDto));
        }

        [Test]
        public async Task CreateAsync_ValidationFails_ReturnsFailure()
        {
            int doctorId = 1;
            int patientId = 1;
            var request = new AppointmentCreateRequestDto { PatientId = patientId, DoctorId = doctorId };

            appointmentValidationMock.Setup(v => v.ValidateAll(request))
                .ReturnsAsync(Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID"));

            var result = await appointmentService.CreateAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DOCTOR_ID"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsSuccess()
        {

        }
    }
}
