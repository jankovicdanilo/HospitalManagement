using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Http
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly TokenStore tokenStore;

        public AuthTokenHandler(TokenStore tokenStore)
        {
            this.tokenStore = tokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(tokenStore.Token))
            {
                requestMessage.Headers.Authorization = AuthenticationHeaderValue.Parse(tokenStore.Token);
            }

            return await base.SendAsync(requestMessage, cancellationToken);
        }
    }
}
