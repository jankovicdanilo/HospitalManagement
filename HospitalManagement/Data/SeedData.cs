using HospitalManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Data
{
    public static class SeedData
    {
        public static async Task SeedAdminAsync(HospitalDbContext dbContext)
        {
            var adminExists = await dbContext.Users.AnyAsync(x => x.Role == UserRole.Admin);

            if (adminExists)
                return;

            var admin = new User
            {
                Username = "admin",
                Email = "admin@hospital.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
            };

            await dbContext.Users.AddAsync(admin);
            dbContext.SaveChanges();

            Console.WriteLine("Default admin seeded successfully.");
        }
    }
}
