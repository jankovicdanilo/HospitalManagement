using HospitalManagement.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddHospitalAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(PolicyNames.ManagePatients, policy =>
                policy.RequireRole(RoleNames.Admin, RoleNames.Manager, RoleNames.Doctor, RoleNames.Mcp));

            options.AddPolicy(PolicyNames.ManageStaff, policy =>
                policy.RequireRole(RoleNames.Admin, RoleNames.Manager, RoleNames.Mcp));

            options.AddPolicy(PolicyNames.AppointmentAccess, policy =>
                policy.RequireRole(RoleNames.Admin, RoleNames.Doctor, RoleNames.Mcp));

            options.AddPolicy(PolicyNames.ReportsAccess, policy =>
                policy.RequireRole(RoleNames.Admin, RoleNames.Manager));
        });

        return services;
    }
}
