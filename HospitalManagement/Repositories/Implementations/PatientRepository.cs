using Dapper;
using HospitalManagement.Data;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HospitalDbContext dbContext;
        private readonly string? connectionString;

        public PatientRepository(HospitalDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.connectionString = configuration.GetConnectionString("HospitalDb");
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

        public async Task<PatientMedicalHistoryDto> GetMedicalHistoryAsync(int patientId)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"
                        SELECT 
                            p.Id,
                            p.Name + ' ' + p.LastName as PatientName,
                            a.Id,
                            a.DateTime,
                            a.Duration,
                            a.Status,
                            a.Notes,
                            d.FirstName + ' ' + d.LastName AS DoctorName,
                            t.Id,
                            t.Description,
                            t.Medication,
                            t.CreatedAt
                        FROM Patient p
                        LEFT JOIN Appointment a ON a.PatientId = p.Id
                        LEFT JOIN Doctor d ON d.Id = a.DoctorId
                        LEFT JOIN Treatment t ON t.AppointmentId = a.Id
                        WHERE p.Id = @PatientId";

            PatientMedicalHistoryDto? patient = null;

            await connection.QueryAsync<PatientMedicalHistoryDto, AppointmentHistoryDto, TreatmentHistoryDto, PatientMedicalHistoryDto>(
                sql,
                (p, a, t) =>
                {
                    if(patient == null)
                    {
                        patient = p;
                    }
                    if(a != null)
                    {
                        a.Treatment = t;
                        patient.Appointments.Add(a);
                    }
                    return patient;
                },
                new { PatientId = patientId},
                splitOn:"Id,Id,Id"
                );

            return patient;
        }
    }
}
