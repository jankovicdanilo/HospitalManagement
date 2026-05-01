using System;
using System.Collections.Generic;

namespace HospitalManagement.Models.Domain;

public partial class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime DateTime { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
