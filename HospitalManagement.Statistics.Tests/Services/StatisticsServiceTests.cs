using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Statistics;
using HospitalManagement.Shared.Models.Enums;
using HospitalManagement.Statistics.Clients.Interfaces;
using HospitalManagement.Statistics.Services.Utility;
using Microsoft.Extensions.Logging;
using Moq;

namespace HospitalManagement.Statistics.Tests.Services
{
    [TestFixture]
    public class StatisticsServiceTests
    {
        private Mock<IAppointmentServiceClient> appointmentClient = null!;
        private Mock<IQueryServiceClient> queryClient = null!;
        private StatisticsBuilder builder = null!;

        private static readonly DateOnly From = new(2026, 9, 1);
        private static readonly DateOnly To = new(2026, 9, 30);

        [SetUp]
        public void SetUp()
        {
            appointmentClient = new Mock<IAppointmentServiceClient>();
            queryClient = new Mock<IQueryServiceClient>();
            builder = new StatisticsBuilder(appointmentClient.Object, queryClient.Object,Mock.Of<ILogger<StatisticsBuilder>>());

            queryClient.Setup(q => q.GetDoctorAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new DoctorResponseDto { FirstName = "Doc", LastName = id.ToString() });
        }

        private void SetupRows(params AppointmentStatsRowDto[] rows) =>
            appointmentClient.Setup(c => c.GetStatsDataAsync(From, To)).ReturnsAsync(rows.ToList());

        private static AppointmentStatsRowDto Row(int doctorId, AppointmentStatus status, decimal cost = 0) =>
            new() { DoctorId = doctorId, Status = status, TotalCost = cost };

        [Test]
        public async Task Load_ExcludesCancelled_AndSortsByCountDescending()
        {
            SetupRows(Row(1, AppointmentStatus.Pending), Row(2, AppointmentStatus.Completed),
                Row(2, AppointmentStatus.Missed), Row(1, AppointmentStatus.Cancelled),
                Row(3, AppointmentStatus.Cancelled));

            var result = await builder.BuildDoctorsLoadAsync(From, To);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Select(d => (d.DoctorId, d.AppointmentCount)),
                Is.EqualTo(new[] { (2, 2), (1, 1) }));
        }

        [Test]
        public async Task Revenue_CountsOnlyCompleted_AndSumsTotalCost()
        {
            SetupRows(Row(1, AppointmentStatus.Completed, 100), Row(1, AppointmentStatus.Completed, 50),
                Row(1, AppointmentStatus.Pending, 999), Row(2, AppointmentStatus.Completed, 70));

            var result = await builder.BuildDoctorsRevenueAsync(From, To);

            Assert.That(result.Data!.Select(d => (d.DoctorId, d.Revenue, d.CompletedCount)),
                Is.EqualTo(new[] { (1, 150m, 2), (2, 70m, 1) }));
        }

        [Test]
        public async Task Load_DoctorNotFound_UsesFallbackName()
        {
            SetupRows(Row(5, AppointmentStatus.Completed));
            queryClient.Setup(q => q.GetDoctorAsync(5)).ReturnsAsync((DoctorResponseDto?)null);

            var result = await builder.BuildDoctorsLoadAsync(From, To);

            Assert.That(result.Data!.Single().DoctorName, Is.EqualTo("Unknown doctor (5)"));
        }

        [Test]
        public async Task Load_FromAfterTo_ReturnsValidationError()
        {
            var result = await builder.BuildDoctorsLoadAsync(To, From);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_DATE_RANGE"));
        }

        [Test]
        public async Task Load_ClientThrows_ReturnsUpstreamFailure()
        {
            appointmentClient.Setup(c => c.GetStatsDataAsync(From, To)).ThrowsAsync(new HttpRequestException());

            var result = await builder.BuildDoctorsLoadAsync(From, To);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("UPSTREAM_FAILURE"));
        }

        [Test]
        public async Task Revenue_ClientThrows_ReturnsUpstreamFailure()
        {
            appointmentClient.Setup(c => c.GetStatsDataAsync(From, To)).ThrowsAsync(new HttpRequestException());

            var result = await builder.BuildDoctorsRevenueAsync(From, To);

            Assert.That(result.ErrorCode, Is.EqualTo("UPSTREAM_FAILURE"));
        }

        [Test]
        public async Task Procedures_GroupsCompletedOnly_AndSortsByRevenue()
        {
            SetupRows(
                new AppointmentStatsRowDto
                {
                    Status = AppointmentStatus.Completed,
                    Procedures = [
                    new() { ProcedureId = 1, ProcedureName = "X-ray", ProcedurePrice = 50 },
            new() { ProcedureId = 2, ProcedureName = "ECG", ProcedurePrice = 30 }]
                },
                new AppointmentStatsRowDto
                {
                    Status = AppointmentStatus.Completed,
                    Procedures = [
                    new() { ProcedureId = 1, ProcedureName = "X-ray", ProcedurePrice = 50 }]
                },
                new AppointmentStatsRowDto
                {
                    Status = AppointmentStatus.Pending,
                    Procedures = [
                    new() { ProcedureId = 2, ProcedureName = "ECG", ProcedurePrice = 999 }]
                });

            var result = await builder.BuildProceduresProfitabilityAsync(From, To);

            Assert.That(result.Data!.Select(p => (p.ProcedureId, p.TimesPerformed, p.Revenue)),
                Is.EqualTo(new[] { (1, 2, 100m), (2, 1, 30m) }));
        }

        [Test]
        public async Task Patients_ComputesTotalsAndTopPatients()
        {
            SetupRows(
                new AppointmentStatsRowDto { PatientId = 1, Status = AppointmentStatus.Completed, TotalCost = 100, DateTime = new(2026, 3, 5) },
                new AppointmentStatsRowDto { PatientId = 1, Status = AppointmentStatus.Completed, TotalCost = 50, DateTime = new(2026, 3, 20) },
                new AppointmentStatsRowDto { PatientId = 2, Status = AppointmentStatus.Pending, TotalCost = 999, DateTime = new(2026, 4, 1) },
                new AppointmentStatsRowDto { PatientId = 3, Status = AppointmentStatus.Cancelled, TotalCost = 999, DateTime = new(2026, 4, 2) });

            queryClient.Setup(q => q.GetPatientAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new PatientResponseDto { Name = "P", LastName = id.ToString() });

            var result = await builder.BuildPatientStatisticsAsync(From, To);

            Assert.That(result.Data!.TotalPatients, Is.EqualTo(2));
            Assert.That(result.Data.TotalVisits, Is.EqualTo(3));
            Assert.That(result.Data.TotalRevenue, Is.EqualTo(150m));
            Assert.That(result.Data.AverageRevenuePerPatient, Is.EqualTo(150m));
            Assert.That(result.Data.TopPatients.First().PatientId, Is.EqualTo(1));
            Assert.That(result.Data.VisitsOverTime.Select(v => (v.Period, v.Visits)),
                Is.EqualTo(new[] { ("2026-03", 2), ("2026-04", 1) }));
        }
    }
}
