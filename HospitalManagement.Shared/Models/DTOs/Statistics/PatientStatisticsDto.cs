using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Models.DTOs.Statistics
{
    public class PatientStatisticsDto
    {
        public int TotalPatients { get; set; }
        public int TotalVisits { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRevenuePerPatient { get; set; }
        public List<VisitsOverTimeDto> VisitsOverTime { get; set; } = [];
        public List<TopPatientDto> TopPatients { get; set; } = [];
    }
}
