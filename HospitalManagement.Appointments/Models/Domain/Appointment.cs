using HospitalManagement.Appointments.Models.Enums;
using System;
using System.Collections.Generic;

namespace HospitalManagement.Appointments.Models.Domain;

public partial class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime DateTime { get; set; }

    public TimeSpan Duration { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public string? Notes { get; set; }

    // Snapshot fields — captured at creation time, since Doctor/Patient
    // now live in the main API's separate database.
    public string PatientName { get; set; } = null!;

    public string PatientEmail { get; set; } = null!;

    public string DoctorName { get; set; } = null!;

    public virtual Treatment? Treatment { get; set; }

    public ICollection<AppointmentProcedure> AppointmentProcedures { get; set; } = [];
}