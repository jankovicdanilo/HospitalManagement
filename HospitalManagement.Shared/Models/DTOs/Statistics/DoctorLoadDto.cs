using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Models.DTOs.Statistics
{
    public class DoctorLoadDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
    }
}
