using HospitalManagement.Appointments.Data;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Appointments.Tests.Repositories
{
    [TestFixture]
    internal class AppointmentRepositoryTests
    {
        private AppointmentDbContext dbContext;
        private AppointmentRepository appointmentRepository;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<AppointmentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            dbContext = new AppointmentDbContext(options);
            appointmentRepository = new AppointmentRepository(dbContext);

            dbContext.Appointments.AddRange(
                new Appointment { Id = 1, DoctorId = 1, PatientId = 1, DateTime = new DateTime(2026, 8, 12, 9, 0, 0), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 2, DoctorId = 1, PatientId = 2, DateTime = new DateTime(2026, 8, 13, 10, 0, 0), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 3, DoctorId = 2, PatientId = 1, DateTime = new DateTime(2026, 8, 14, 11, 0, 0), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 4, DoctorId = 2, PatientId = 2, DateTime = new DateTime(2026, 8, 16, 14, 0, 0), Duration = TimeSpan.FromMinutes(30) }
            );

            await dbContext.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetAllAsync_StartAndEndDateProvided_ReturnsOnlyAppointmentsInRange()
        {
            var filter = new AppointmentFilterDto
            {
                StartDate = new DateOnly(2026, 8, 13),
                EndDate = new DateOnly(2026, 8, 14),
                PageNumber = 1,
                PageSize = 20
            };

            var (items, totalCount) = await appointmentRepository.GetAllAsync(filter);

            Assert.That(totalCount, Is.EqualTo(2));
            Assert.That(items.Select(x => x.Id), Is.EquivalentTo(new[] { 2, 3 }));
        }

        [Test]
        public async Task GetAllAsync_StartDateEqualsEndDate_BehavesLikeSingleDayFilter()
        {
            var filter = new AppointmentFilterDto
            {
                StartDate = new DateOnly(2026, 8, 12),
                EndDate = new DateOnly(2026, 8, 12),
                PageNumber = 1,
                PageSize = 20
            };

            var (items, totalCount) = await appointmentRepository.GetAllAsync(filter);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllAsync_OnlyDateProvided_FallsBackToExactDateMatch()
        {
            var filter = new AppointmentFilterDto
            {
                Date = new DateOnly(2026, 8, 16),
                PageNumber = 1,
                PageSize = 20
            };

            var (items, totalCount) = await appointmentRepository.GetAllAsync(filter);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items[0].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task GetAllAsync_RangeAndDateBothProvided_RangeTakesPriority()
        {
            var filter = new AppointmentFilterDto
            {
                Date = new DateOnly(2026, 8, 12),
                StartDate = new DateOnly(2026, 8, 13),
                EndDate = new DateOnly(2026, 8, 14),
                PageNumber = 1,
                PageSize = 20
            };

            var (items, totalCount) = await appointmentRepository.GetAllAsync(filter);

            Assert.That(totalCount, Is.EqualTo(2));
            Assert.That(items.Select(x => x.Id), Is.EquivalentTo(new[] { 2, 3 }));
        }

        [Test]
        public async Task GetAllAsync_RangeOutsideAnyData_ReturnsEmpty()
        {
            var filter = new AppointmentFilterDto
            {
                StartDate = new DateOnly(2026, 9, 1),
                EndDate = new DateOnly(2026, 9, 7),
                PageNumber = 1,
                PageSize = 20
            };

            var (items, totalCount) = await appointmentRepository.GetAllAsync(filter);

            Assert.That(totalCount, Is.EqualTo(0));
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task GetTopDoctorIdsByAppointmentCountAsync_ReturnsDoctorsOrderedByAppointmentCountDescending()
        {
            dbContext.Appointments.AddRange(
                new Appointment { Id = 101, DoctorId = 501, PatientId = 501, DateTime = new DateTime(2026, 8, 1), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 102, DoctorId = 501, PatientId = 502, DateTime = new DateTime(2026, 8, 2), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 103, DoctorId = 501, PatientId = 503, DateTime = new DateTime(2026, 8, 3), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 104, DoctorId = 501, PatientId = 504, DateTime = new DateTime(2026, 8, 4), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 105, DoctorId = 501, PatientId = 505, DateTime = new DateTime(2026, 8, 5), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 106, DoctorId = 502, PatientId = 501, DateTime = new DateTime(2026, 8, 6), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 107, DoctorId = 502, PatientId = 502, DateTime = new DateTime(2026, 8, 7), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 108, DoctorId = 502, PatientId = 503, DateTime = new DateTime(2026, 8, 8), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 109, DoctorId = 502, PatientId = 504, DateTime = new DateTime(2026, 8, 9), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 110, DoctorId = 503, PatientId = 501, DateTime = new DateTime(2026, 8, 10), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 111, DoctorId = 503, PatientId = 502, DateTime = new DateTime(2026, 8, 11), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 112, DoctorId = 503, PatientId = 503, DateTime = new DateTime(2026, 8, 12), Duration = TimeSpan.FromMinutes(30) }
            );
            await dbContext.SaveChangesAsync();

            var result = await appointmentRepository.GetTopDoctorIdsByAppointmentCountAsync(3);

            Assert.That(result, Is.EqualTo(new List<int> { 501, 502, 503 }));
        }

        [Test]
        public async Task GetTopDoctorIdsByAppointmentCountAsync_RespectsCountLimit()
        {
            dbContext.Appointments.AddRange(
                new Appointment { Id = 113, DoctorId = 601, PatientId = 501, DateTime = new DateTime(2026, 8, 1), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 114, DoctorId = 601, PatientId = 502, DateTime = new DateTime(2026, 8, 2), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 115, DoctorId = 601, PatientId = 503, DateTime = new DateTime(2026, 8, 3), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 116, DoctorId = 602, PatientId = 501, DateTime = new DateTime(2026, 8, 4), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 117, DoctorId = 602, PatientId = 502, DateTime = new DateTime(2026, 8, 5), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 118, DoctorId = 602, PatientId = 503, DateTime = new DateTime(2026, 8, 6), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 119, DoctorId = 603, PatientId = 501, DateTime = new DateTime(2026, 8, 7), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 120, DoctorId = 603, PatientId = 502, DateTime = new DateTime(2026, 8, 8), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 121, DoctorId = 603, PatientId = 503, DateTime = new DateTime(2026, 8, 9), Duration = TimeSpan.FromMinutes(30) }
            );
            await dbContext.SaveChangesAsync();

            var result = await appointmentRepository.GetTopDoctorIdsByAppointmentCountAsync(2);

            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetTopPatientIdsByAppointmentCountAsync_ReturnsPatientsOrderedByAppointmentCountDescending()
        {
            dbContext.Appointments.AddRange(
                new Appointment { Id = 122, DoctorId = 501, PatientId = 701, DateTime = new DateTime(2026, 8, 1), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 123, DoctorId = 502, PatientId = 701, DateTime = new DateTime(2026, 8, 2), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 124, DoctorId = 503, PatientId = 701, DateTime = new DateTime(2026, 8, 3), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 125, DoctorId = 501, PatientId = 701, DateTime = new DateTime(2026, 8, 4), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 126, DoctorId = 502, PatientId = 702, DateTime = new DateTime(2026, 8, 5), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 127, DoctorId = 501, PatientId = 702, DateTime = new DateTime(2026, 8, 6), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 128, DoctorId = 503, PatientId = 702, DateTime = new DateTime(2026, 8, 7), Duration = TimeSpan.FromMinutes(30) },
                new Appointment { Id = 129, DoctorId = 501, PatientId = 703, DateTime = new DateTime(2026, 8, 8), Duration = TimeSpan.FromMinutes(30) }
            );
            await dbContext.SaveChangesAsync();

            var result = await appointmentRepository.GetTopPatientIdsByAppointmentCountAsync(2);

            Assert.That(result, Is.EqualTo(new List<int> { 701, 702 }));
        }
    }
}
