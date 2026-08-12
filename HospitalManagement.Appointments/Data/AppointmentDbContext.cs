using HospitalManagement.Appointments.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HospitalManagement.Appointments.Data;

public partial class AppointmentDbContext : DbContext
{
    public AppointmentDbContext()
    {
    }

    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentProcedure> AppointmentProcedures { get; set; }

    public virtual DbSet<Treatment> Treatments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointment");

            entity.Property(e => e.DateTime)
                .HasColumnType("datetime")
                .HasConversion(utcConverter);

            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50).HasConversion<string>();
        });

        modelBuilder.Entity<AppointmentProcedure>(entity =>
        {
            entity.HasKey(ap => new { ap.AppointmentId, ap.ProcedureId });

            entity.HasOne(ap => ap.Appointment)
                  .WithMany(a => a.AppointmentProcedures)
                  .HasForeignKey(ap => ap.AppointmentId);

            entity.Property(e => e.ProcedureName).HasMaxLength(200);
            entity.Property(e => e.ProcedurePrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.ToTable("Treatment");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Medication).HasMaxLength(500);

            entity.HasOne(t => t.Appointment)
                    .WithOne(a => a.Treatment)
                    .HasForeignKey<Treatment>(t => t.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}