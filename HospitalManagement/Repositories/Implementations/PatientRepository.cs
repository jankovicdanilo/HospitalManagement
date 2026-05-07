using HospitalManagement.Data;
using HospitalManagement.Models.Domain;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HospitalDbContext dbContext;

        public PatientRepository(HospitalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Patient>> GetAllAsync()
        {
           return await dbContext.Patients.AsNoTracking().ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
