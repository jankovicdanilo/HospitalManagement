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

        public bool EmailExists(string email)
        {
            var patient = dbContext.Patients.FirstOrDefault(x => x.Email == email);

            return patient != null;
        }

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);
        }

        public bool PatientExists(int id)
        {
            var patient = dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);

            if(patient == null)
            {
                return false;
            }

            return true;
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {
            dbContext.Patients.Update(patient);
            await dbContext.SaveChangesAsync();

            return patient;
        }

        public async Task<Patient?> Delete(int id)
        {
            var patient = await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);

            if(patient == null)
            {
                return null;
            }

            dbContext.Patients.Remove(patient);
            dbContext.SaveChanges();

            return patient;
        }

        public async Task<List<Patient>> GetAllAsync()
        {
            return await dbContext.Patients.AsNoTracking().ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);
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
