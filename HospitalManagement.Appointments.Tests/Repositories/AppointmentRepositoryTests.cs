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
    }
}
