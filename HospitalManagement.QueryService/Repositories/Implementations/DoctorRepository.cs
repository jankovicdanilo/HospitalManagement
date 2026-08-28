using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Shared.Extensions;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalManagementDbContext dbContext;

        public DoctorRepository(HospitalManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<(List<Doctor> items, int totalCount)> GetAllAsync(DoctorFilterDto filter)
        {
            var query = dbContext.Doctors.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(x => x.FirstName.Contains(filter.Search) || x.LastName.Contains(filter.Search));
            }

            return await query.ToPagedResultAsync(x => x.Id, filter.PageNumber, filter.PageSize);
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}