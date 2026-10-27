using HospitalManagement.Shared.Models.Enums;

namespace HospitalManagement.Shared.Models.DTOs.Statistics
{
    public class AppointmentStatsRowDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime DateTime { get; set; } // UTC
        public AppointmentStatus Status { get; set; }
        public decimal TotalCost { get; set; }
        public List<AppointmentStatsProcedureDto> Procedures { get; set; } = [];
    }
}

