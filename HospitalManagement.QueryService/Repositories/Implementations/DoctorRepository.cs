using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Extensions;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalManagementDbContext dbContext;
        private readonly ILogger<DoctorRepository> logger;

        public DoctorRepository(HospitalManagementDbContext dbContext, ILogger<DoctorRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
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

        public async Task<List<Doctor>> GetByIdsAsync(List<int> ids)
        {
            logger.LogInformation("GetByIdsAsync called with ids: {Ids}", string.Join(",", ids));
            var result = await dbContext.Doctors.Where(x => ids.Contains(x.Id)).ToListAsync();
            logger.LogInformation("GetByIdsAsync query returned {Count} rows", result.Count);
            return result;
        }
    }
}