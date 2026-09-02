using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Extensions;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Patient;
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

        public async Task<(List<Patient> items, int totalCount)> GetAllAsync(PatientFilterDto filter)
        {
            var query = dbContext.Patients.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(x => x.Name.Contains(filter.Search) || x.LastName.Contains(filter.Search));
            }

            return await query.ToPagedResultAsync(x => x.Id, filter.PageNumber, filter.PageSize);
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