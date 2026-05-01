using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByUsernameAsync(string username);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> CreateAsync(User user);

        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        Task Delete(int id);

        Task<User> UpdateAsync(User user);
    }
}
