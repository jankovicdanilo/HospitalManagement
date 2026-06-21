using HospitalManagement.QueryService.Data;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly QueryDbContext dbContext;

        public DoctorRepository(QueryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<DoctorReadModel>> GetAllAsync()
        {
            return await dbContext.Doctors.AsNoTracking().ToListAsync();
        }

        public async Task<DoctorReadModel?> GetByIdAsync(int id)
        {
            return await dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}