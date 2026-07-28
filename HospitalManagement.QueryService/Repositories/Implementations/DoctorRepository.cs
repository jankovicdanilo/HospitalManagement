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

        public async Task<(List<Doctor> items, int totalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = dbContext.Doctors.AsNoTracking().OrderBy(x => x.Id);

            var totalCount = await query.CountAsync();
            var offset = (pageNumber - 1) * pageSize;

            var items = await query.Skip(offset).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}