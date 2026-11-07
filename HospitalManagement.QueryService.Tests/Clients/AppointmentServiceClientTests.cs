using HospitalManagement.QueryService.Clients;
using HospitalManagement.QueryService.Clients.Implementations;
using HospitalManagement.QueryService.Clients.Interfaces;
using HospitalManagement.QueryService.Tests.Utility;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;

namespace HospitalManagement.QueryService.Tests.Clients
{
    [TestFixture]
    internal class AppointmentServiceClientTests
    {
        private Mock<ILogger<AppointmentServiceClient>> loggerMock;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<AppointmentServiceClient>>();
        }

        private AppointmentServiceClient CreateClient(StubHandler handler)
        {
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") };
            return new AppointmentServiceClient(httpClient, loggerMock.Object);
        }

        private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json) =>
            new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

        [Test]
        public async Task GetPatientHistoryAsync_Returns200WithAppointments_ReturnsOkWithMappedHistory()
        {
            var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK,
                 "[{\"id\":1,\"doctorName\":\"Dr. Smith\"},{\"id\":2,\"doctorName\":\"Dr. Jones\"}]"));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.Ok));
            Assert.That(result.History!.PatientId, Is.EqualTo(5));
            Assert.That(result.History.Appointments.Count(), Is.EqualTo(2));
            Assert.That(result.History.Appointments.First().Id, Is.EqualTo(1));
            Assert.That(result.History.Appointments.First().DoctorName, Is.EqualTo("Dr. Smith"));
            Assert.That(handler.LastRequest!.RequestUri!.AbsolutePath, Is.EqualTo("/api/appointment/patient/5/history"));
        }

        [Test]
        public async Task GetPatientHistoryAsync_Returns200WithEmptyList_ReturnsOkWithNoAppointments()
        {
            var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, "[]"));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.Ok));
            Assert.That(result.History!.Appointments.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetPatientHistoryAsync_Returns403WithBody_ReturnsForbiddenWithCodeAndMessage()
        {
            var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.Forbidden,
                "{\"errorCode\":\"NOT_YOUR_PATIENT\",\"message\":\"You can only see your own patients\"}"));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.Forbidden));
            Assert.That(result.ErrorCode, Is.EqualTo("NOT_YOUR_PATIENT"));
            Assert.That(result.Message, Is.EqualTo("You can only see your own patients"));
            Assert.That(result.History, Is.Null);
        }

        [Test]
        public async Task GetPatientHistoryAsync_Returns403WithEmptyBody_ReturnsForbiddenWithDefaults()
        {
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.Forbidden));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.Forbidden));
            Assert.That(result.ErrorCode, Is.EqualTo("NOT_YOUR_PATIENT"));
            Assert.That(result.Message, Is.EqualTo("You can only see your own patients"));
        }

        [Test]
        public async Task GetPatientHistoryAsync_Returns404_ReturnsNotFound()
        {
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.NotFound));
            Assert.That(result.History, Is.Null);
        }

        [Test]
        public async Task GetPatientHistoryAsync_Returns500_ReturnsFailed()
        {
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.Failed));
            Assert.That(result.History, Is.Null);
        }

        [Test]
        public async Task GetPatientHistoryAsync_Returns200WithNullBody_ReturnsFailed()
        {
            var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, "null"));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.Failed));
        }

        [Test]
        public async Task GetPatientHistoryAsync_Returns200WithInvalidJson_ReturnsFailed()
        {
            var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, "this is not json"));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.Failed));
        }

        [Test]
        public async Task GetPatientHistoryAsync_ConnectionFails_ReturnsFailed()
        {
            var handler = new StubHandler(_ => throw new HttpRequestException("connection refused"));
            var client = CreateClient(handler);

            var result = await client.GetPatientHistoryAsync(5);

            Assert.That(result.Status, Is.EqualTo(HistoryFetchStatus.Failed));
        }
    }
}
