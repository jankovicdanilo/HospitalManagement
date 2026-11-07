using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Security;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace HospitalManagement.Appointments.Tests.Services
{
    [TestFixture]
    internal class AppointmentAccessTests
    {
        private Mock<IAppointmentRepository> appointmentRepositoryMock;
        private Mock<ILogger<AppointmentAccess>> loggerMock;
        private AppointmentAccess appointmentAccess;

        [SetUp]
        public void SetUp()
        {
            appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            loggerMock = new Mock<ILogger<AppointmentAccess>>();

            appointmentAccess = new AppointmentAccess(appointmentRepositoryMock.Object, loggerMock.Object);
        }

        private static ClaimsPrincipal CreateUser(string role, string? doctorId = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Role, role)
            };

            if (doctorId != null)
            {
                claims.Add(new Claim(AppClaims.DoctorId, doctorId));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        [TestCase(RoleNames.Admin)]
        [TestCase(RoleNames.Mcp)]
        public void GetDoctorScope_AdminOrMcp_ReturnsUnrestricted(string role)
        {
            var result = appointmentAccess.GetDoctorScope(CreateUser(role));

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public void GetDoctorScope_DoctorWithDoctorId_ReturnsThatDoctor()
        {
            var result = appointmentAccess.GetDoctorScope(CreateUser(RoleNames.Doctor, "6"));

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(6));
        }

        [TestCase(null)]
        [TestCase("abc")]
        public void GetDoctorScope_DoctorWithoutValidDoctorId_ReturnsDoctorNotLinked(string? doctorId)
        {
            var result = appointmentAccess.GetDoctorScope(CreateUser(RoleNames.Doctor, doctorId));

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Forbidden));
            Assert.That(result.ErrorCode, Is.EqualTo("DOCTOR_NOT_LINKED"));
        }

        [TestCase(RoleNames.Manager)]
        [TestCase("Guest")]
        public void GetDoctorScope_RoleWithoutAppointmentAccess_ReturnsForbidden(string role)
        {
            var result = appointmentAccess.GetDoctorScope(CreateUser(role));

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Forbidden));
            Assert.That(result.ErrorCode, Is.EqualTo("FORBIDDEN"));
        }

        [Test]
        public void CheckDoctor_DoctorActsForHimself_ReturnsOk()
        {
            var result = appointmentAccess.CheckDoctor(CreateUser(RoleNames.Doctor, "6"), 6);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void CheckDoctor_DoctorActsForAnotherDoctor_ReturnsForbidden()
        {
            var result = appointmentAccess.CheckDoctor(CreateUser(RoleNames.Doctor, "6"), 7);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Forbidden));
            Assert.That(result.ErrorCode, Is.EqualTo("NOT_YOUR_APPOINTMENT"));
        }

        [Test]
        public void CheckDoctor_AdminActsForAnyDoctor_ReturnsOk()
        {
            var result = appointmentAccess.CheckDoctor(CreateUser(RoleNames.Admin), 7);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void CheckDoctor_DoctorWithoutDoctorId_ReturnsDoctorNotLinked()
        {
            var result = appointmentAccess.CheckDoctor(CreateUser(RoleNames.Doctor), 6);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("DOCTOR_NOT_LINKED"));
        }

        [Test]
        public async Task CheckAppointmentAsync_Admin_ReturnsOkWithoutLoadingTheAppointment()
        {
            var result = await appointmentAccess.CheckAppointmentAsync(CreateUser(RoleNames.Admin), 10);

            Assert.That(result.Success, Is.True);
            appointmentRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task CheckAppointmentAsync_DoctorOwnsAppointment_ReturnsOk()
        {
            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new Appointment { Id = 10, DoctorId = 6 });

            var result = await appointmentAccess.CheckAppointmentAsync(CreateUser(RoleNames.Doctor, "6"), 10);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task CheckAppointmentAsync_AppointmentBelongsToAnotherDoctor_ReturnsForbidden()
        {
            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new Appointment { Id = 10, DoctorId = 7 });

            var result = await appointmentAccess.CheckAppointmentAsync(CreateUser(RoleNames.Doctor, "6"), 10);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Forbidden));
            Assert.That(result.ErrorCode, Is.EqualTo("NOT_YOUR_APPOINTMENT"));
        }

        [Test]
        public async Task CheckAppointmentAsync_AppointmentDoesNotExist_ReturnsOkSoTheServiceCanAnswerNotFound()
        {
            appointmentRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Appointment?)null);

            var result = await appointmentAccess.CheckAppointmentAsync(CreateUser(RoleNames.Doctor, "6"), 10);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task CheckPatientAsync_DoctorHasAppointmentWithPatient_ReturnsOk()
        {
            appointmentRepositoryMock.Setup(r => r.GetByPatientIdAsync(5)).ReturnsAsync(new List<Appointment>
            {
                new Appointment { Id = 1, PatientId = 5, DoctorId = 7 },
                new Appointment { Id = 2, PatientId = 5, DoctorId = 6 }
            });

            var result = await appointmentAccess.CheckPatientAsync(CreateUser(RoleNames.Doctor, "6"), 5);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task CheckPatientAsync_PatientOnlySeenByOtherDoctors_ReturnsForbidden()
        {
            appointmentRepositoryMock.Setup(r => r.GetByPatientIdAsync(5)).ReturnsAsync(new List<Appointment>
            {
                new Appointment { Id = 1, PatientId = 5, DoctorId = 7 }
            });

            var result = await appointmentAccess.CheckPatientAsync(CreateUser(RoleNames.Doctor, "6"), 5);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Forbidden));
            Assert.That(result.ErrorCode, Is.EqualTo("NOT_YOUR_PATIENT"));
        }

        [Test]
        public async Task CheckPatientAsync_PatientHasNoAppointments_ReturnsForbiddenForDoctor()
        {
            appointmentRepositoryMock.Setup(r => r.GetByPatientIdAsync(5)).ReturnsAsync(new List<Appointment>());

            var result = await appointmentAccess.CheckPatientAsync(CreateUser(RoleNames.Doctor, "6"), 5);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("NOT_YOUR_PATIENT"));
        }

        [Test]
        public async Task CheckPatientAsync_Admin_ReturnsOkWithoutLoadingAppointments()
        {
            var result = await appointmentAccess.CheckPatientAsync(CreateUser(RoleNames.Admin), 5);

            Assert.That(result.Success, Is.True);
            appointmentRepositoryMock.Verify(r => r.GetByPatientIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task CheckPatientAsync_Manager_ReturnsForbiddenWithoutLoadingAppointments()
        {
            var result = await appointmentAccess.CheckPatientAsync(CreateUser(RoleNames.Manager), 5);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("FORBIDDEN"));
            appointmentRepositoryMock.Verify(r => r.GetByPatientIdAsync(It.IsAny<int>()), Times.Never);
        }
    }
}