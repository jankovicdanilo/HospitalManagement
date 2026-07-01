using Dapper;
using HospitalManagement.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
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

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<Patient?> CreateAsync(Patient patient)
        {
            await dbContext.Patients.AddAsync(patient);

            await dbContext.SaveChangesAsync();

            return patient;
        }

        public async Task<Patient?> GetByEmail(string email)
        {
            return await dbContext.Patients.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await dbContext.Patients.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> PatientExists(int id)
        {
            return await dbContext.Patients.AnyAsync(x => x.Id == id);
        }

        public async Task<Patient?> GetMedicalHistoryAsync(int patientId)
        {
            // TODO: cross-service call to appointment microservice needed
            await Task.CompletedTask;
            return null;
        }
    }
}
