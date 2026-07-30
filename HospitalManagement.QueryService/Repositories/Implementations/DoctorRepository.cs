using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Shared.Extensions;

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
            return await dbContext.Doctors.AsNoTracking().ToPagedResultAsync(x => x.Id, pageNumber, pageSize);
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}