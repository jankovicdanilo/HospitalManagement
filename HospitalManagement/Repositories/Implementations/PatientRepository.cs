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

        public async Task<Patient?> CreateAsync(Patient patient)
        {
            await dbContext.Patients.AddAsync(patient);
            await dbContext.SaveChangesAsync();

            return patient;
        }

        public async Task<Patient> GetByEmail(string email)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
