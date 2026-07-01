using HospitalManagement.QueryService.Models.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Data
{
    public class QueryDbContext : DbContext
    {
        public QueryDbContext(DbContextOptions<QueryDbContext> options)
            : base(options)
        {
        }

        public DbSet<DoctorReadModel> Doctors { get; set; }
        public DbSet<PatientReadModel> Patients { get; set; }
        public DbSet<DoctorScheduleReadModel> DoctorSchedules { get; set; }
        public DbSet<ProcedureReadModel> Procedures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorReadModel>(entity =>
            {
                entity.ToTable("Doctor");
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Specialization).HasMaxLength(100);
            });

            modelBuilder.Entity<PatientReadModel>(entity =>
            {
                entity.ToTable("Patient");
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<DoctorScheduleReadModel>(entity =>
            {
                entity.ToTable("DoctorSchedule");
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.DayOfWeek).HasConversion<string>();
            });

            modelBuilder.Entity<ProcedureReadModel>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(p => p.Price).HasPrecision(18, 2);
            });
        }
    }
}