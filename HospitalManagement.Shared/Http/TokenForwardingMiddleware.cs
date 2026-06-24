using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Http
{
    public class TokenForwardingMiddleware
    {
        private readonly RequestDelegate next;

        public TokenForwardingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, TokenStore tokenStore)
        {
            var token = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                tokenStore.Token = token;
            }

            await next(context);
        }
    }
}
