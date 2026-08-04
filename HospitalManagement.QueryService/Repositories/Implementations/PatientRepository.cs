using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Shared.Extensions;

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
            return await dbContext.Patients.AsNoTracking().ToPagedResultAsync(x => x.Id, pageNumber, pageSize);
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