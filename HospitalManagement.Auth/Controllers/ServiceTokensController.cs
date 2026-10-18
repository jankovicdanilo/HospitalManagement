using FluentValidation;
using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Auth.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Auth.Controllers
{
    [Route("api/auth/service-tokens")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ServiceTokensController : BaseController
    {
        private readonly IServiceTokenService serviceTokenService;

        public ServiceTokensController(IServiceTokenService serviceTokenService)
        {
            this.serviceTokenService = serviceTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ServiceTokenCreateRequestDto request,
            [FromServices] IValidator<ServiceTokenCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);
            if(!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await serviceTokenService.CreateAsync(request, User.Identity?.Name ?? "unknown");

            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await serviceTokenService.GetAllAsync();

            return result.Success ? Ok(result) : HandleFailure(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> RevokeAsync([FromRoute] int id)
        {
            var result = await serviceTokenService.RevokeAsync(id);

            return result.Success ? Ok(new { result.Message }) : HandleFailure(result);
        }
    }
}
