using HospitalManagement.Auth.Models.Domain;
using HospitalManagement.Auth.Repositories.Interfaces;
using HospitalManagement.Auth.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Auth.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDbContext dbContext;

        public AuthRepository(AuthDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User?> CreateAsync(User user)
        {
            await dbContext.Users.AddAsync(user);

            await dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Delete(int id)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            dbContext.Users.Remove(user);

            await dbContext.SaveChangesAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            dbContext.Users.Update(user);

            await dbContext.SaveChangesAsync();

            return user;
        }
    }
}
