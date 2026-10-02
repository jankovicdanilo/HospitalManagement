using Polly;
using Polly.Extensions.Http;

namespace HospitalManagement.Statistics.Utility
{
    public static class HttpPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> Retry() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)));

        public static IAsyncPolicy<HttpResponseMessage> CircuitBreaker() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        public static IAsyncPolicy<HttpResponseMessage> Timeout() =>
            Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));

        public static IHttpClientBuilder AddStandardResilience(this IHttpClientBuilder builder) =>
            builder
                .AddPolicyHandler(Retry())
                .AddPolicyHandler(CircuitBreaker())
                .AddPolicyHandler(Timeout());
    }
}
