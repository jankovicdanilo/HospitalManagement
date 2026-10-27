using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Models.DTOs.Statistics
{
    public class DoctorStatisticsDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
        public int PendingCount { get; set; }
        public int CompletedCount { get; set; }
        public int MissedCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal Revenue { get; set; }
        public int UniquePatients { get; set; }
        public List<VisitsOverTimeDto> VisitsOverTime { get; set; } = [];
        public List<ProcedureProfitabilityDto> TopProcedures { get; set; } = [];
        public List<TopPatientDto> TopPatients { get; set; } = [];
    }
}
