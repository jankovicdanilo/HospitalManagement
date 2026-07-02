using HospitalManagement.Shared.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HospitalManagement.Shared.Data
{
    public class HospitalManagementDbContext : DbContext
    {
        public HospitalManagementDbContext(DbContextOptions<HospitalManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Procedure> Procedures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Specialization).HasMaxLength(100);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<DoctorSchedule>(entity =>
            {
                entity.ToTable("DoctorSchedule");
                entity.HasOne(d => d.Doctor)
                    .WithMany(d => d.Schedules)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.DayOfWeek).HasConversion<string>();
            });

            modelBuilder.Entity<Procedure>(entity =>
            {
                entity.Property(p => p.Price).HasPrecision(18, 2);
            });
        }
    }
}