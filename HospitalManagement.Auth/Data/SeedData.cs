using HospitalManagement.Auth.Models.Domain;
using HospitalManagement.Auth.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Auth.Data
{
    public static class SeedData
    {
        public static async Task SeedAdminAsync(AuthDbContext dbContext, IConfiguration configuration,
            IHostEnvironment environment, ILogger logger)
        {
            if (await dbContext.Users.AnyAsync(x => x.Role == UserRole.Admin))
            {
                return;
            }

            var password = configuration["Admin:Password"];
            if (string.IsNullOrWhiteSpace(password))
            {
                if (!environment.IsDevelopment())
                {
                    throw new InvalidOperationException("Admin:Password must be set to create the first admin account.");
                }

                logger.LogWarning("Admin:Password is not set, using the development default password");
                password = "Admin123!";
            }

            var admin = new User
            {
                Username = configuration["Admin:Username"] ?? "admin",
                Email = configuration["Admin:Email"] ?? "admin@hospital.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = UserRole.Admin,
            };

            await dbContext.Users.AddAsync(admin);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Admin account {Username} created", admin.Username);
        }
    }
}
