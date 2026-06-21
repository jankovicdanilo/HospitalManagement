using HospitalManagement.QueryService.Data;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly QueryDbContext dbContext;

        public PatientRepository(QueryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<PatientReadModel>> GetAllAsync()
        {
            return await dbContext.Patients.AsNoTracking().ToListAsync();
        }

        public async Task<PatientReadModel?> GetByIdAsync(int id)
        {
            return await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PatientReadModel?> GetByEmailAsync(string email)
        {
            return await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}