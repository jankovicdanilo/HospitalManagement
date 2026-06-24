using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Shared.Models.DTOs.External;
using System;
using System.Collections.Generic;

namespace HospitalManagement.Appointments.Models.Domain;

public partial class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public ExternalPatientDto? Patient { get; set; } //not mapped to db

    public int DoctorId { get; set; }

    public ExternalDoctorDto? Doctor { get; set; } //not mapped to db

    public DateTime DateTime { get; set; }

    public TimeSpan Duration { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public string? Notes { get; set; }

    public virtual Treatment? Treatment { get; set; }

    public ICollection<AppointmentProcedure> AppointmentProcedures { get; set; } = [];
}