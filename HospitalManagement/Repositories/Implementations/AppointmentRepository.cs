using Dapper;
using HospitalManagement.Common;
using HospitalManagement.Data;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Models.Enums;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace HospitalManagement.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly HospitalDbContext dbContext;
        private readonly string? connectionString;

        public AppointmentRepository(HospitalDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            connectionString = configuration.GetConnectionString("HospitalDb");
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await dbContext.Appointments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Appointment?> Delete(int id)
        {
            var appointment = await dbContext.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if(appointment == null)
            {
                return null;
            }

            dbContext.Appointments.Remove(appointment);
            await dbContext.SaveChangesAsync();

            return appointment;
        }

        

        public async Task<List<Appointment>?> GetByDoctorIdAsync(int id)
        {
            return await dbContext.Appointments.AsNoTracking().Where(x => x.DoctorId == id).ToListAsync();
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            dbContext.Appointments.Update(appointment);
            await dbContext.SaveChangesAsync();

            return appointment;
        }

        public async Task<List<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateOnly date)
        {
            return await dbContext.Appointments.Where(x => x.DoctorId == doctorId && 
                DateOnly.FromDateTime(x.DateTime) == date)
                .ToListAsync();
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            await dbContext.Appointments.AddAsync(appointment);
            await dbContext.SaveChangesAsync();

            return appointment;
        }

        public async Task<PagedResult<AppointmentListResponseDto>> GetAllAsync(AppointmentFilterDto filter)
        {
            using var connection = new SqlConnection(connectionString);

            var offset = (filter.PageNumber - 1) * filter.PageSize;

            var sql = @"
                WITH Filtered AS(
                    SELECT 
                        a.Id, 
                        a.DateTime, 
                        a.Duration, 
                        a.Status, 
                        a.Notes, 
                        d.FirstName + ' ' + d.LastName AS DoctorName,
                        p.Name + ' ' + p.Lastname AS PatientName 
                    FROM Appointment a 
                    JOIN Doctor d  ON d.Id = a.DoctorId
                    JOIN Patient p ON p.Id = a.PatientId 
                    WHERE
                        (@DoctorId IS NULL OR a.DoctorId = @DoctorId) AND
                        (@PatientId IS NULL OR a.PatientId = @PatientId) AND
                        (@Date IS NULL OR cast(a.DateTime AS DATE) = @Date) AND
                        (@Status IS NULL OR a.Status = @Status)
                )
                SELECT
                    COUNT(*) OVER() AS TotalCount,
                    Id,
                    DateTime,
                    Duration,
                    Status,
                    Notes,
                    DoctorName,
                    PatientName
                FROM Filtered
                ORDER BY Id
                OFFSET @offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var result = await connection.QueryAsync<AppointmentListResponseDto>(sql, new
            {
                DoctorId = filter.DoctorId,
                PatientId = filter.PatientId,
                Date = filter.Date.HasValue ? filter.Date.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                Status = filter.Status.HasValue ? filter.Status.Value.ToString() : null,
                offset,
                PageSize = filter.PageSize
            });

            var items = result.ToList();
            var totalCount = items.FirstOrDefault()?.TotalCount ?? 0;

            return PagedResult<AppointmentListResponseDto>.Create(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<Appointment>> GetPendingPastAppointmentsAsync()
        {
            var now = DateTime.UtcNow;
            var appointments = await dbContext.Appointments
                .Where(a => a.Status == AppointmentStatus.Pending && a.DateTime < now)
                .ToListAsync();

            return appointments.Where(a => a.DateTime.Add(a.Duration).AddHours(1) < now);
        }
    }
}
