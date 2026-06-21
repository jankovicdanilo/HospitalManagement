using HospitalManagement.CommandService.Data;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.CommandService.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly CommandDbContext dbContext;

        public PatientRepository(CommandDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Patient?> CreateAsync(Patient patient)
        {
            await dbContext.Patients.AddAsync(patient);
            await dbContext.SaveChangesAsync();
            return patient;
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
            if (patient == null) return null;
            dbContext.Patients.Remove(patient);
            await dbContext.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> EmailExists(string email)
        {
            return await dbContext.Patients.AnyAsync(x => x.Email == email);
        }

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> PatientExists(int id)
        {
            return await dbContext.Patients.AnyAsync(x => x.Id == id);
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}