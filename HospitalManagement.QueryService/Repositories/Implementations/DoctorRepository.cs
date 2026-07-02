using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalManagementDbContext dbContext;

        public DoctorRepository(HospitalManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Doctor>> GetAllAsync()
        {
            return await dbContext.Doctors.AsNoTracking().ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}