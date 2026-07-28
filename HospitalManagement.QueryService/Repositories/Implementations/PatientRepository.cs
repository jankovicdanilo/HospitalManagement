using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HospitalManagementDbContext dbContext;

        public PatientRepository(HospitalManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<(List<Patient> items, int totalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = dbContext.Patients.AsNoTracking().OrderBy(x => x.Id);

            var totalCount = await query.CountAsync();
            var offset = (pageNumber -1) * pageSize;

            var items = await query.Skip(offset).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}