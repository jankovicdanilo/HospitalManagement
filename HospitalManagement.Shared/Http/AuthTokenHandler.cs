using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Http
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            Console.WriteLine($"AuthTokenHandler fired. HttpContext: " +
                $"{(httpContextAccessor.HttpContext == null ? "NULL" : "OK")}, Token: " +
                $"{(string.IsNullOrEmpty(token) ? "EMPTY" : "PRESENT")}");

            if (!string.IsNullOrEmpty(token))
            {
                requestMessage.Headers.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(token);
            }

            return await base.SendAsync(requestMessage, cancellationToken);
        }
    }
}
