using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Models.DTOs.Statistics
{
    public class DoctorLoadTimelineDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
        public List<VisitsOverTimeDto> Points { get; set; } = [];
    }
}
