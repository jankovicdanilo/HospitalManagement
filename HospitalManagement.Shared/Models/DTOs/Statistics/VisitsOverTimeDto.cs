using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Models.DTOs.Statistics
{
    public class VisitsOverTimeDto
    {
        public string Period { get; set; } = string.Empty; // "2026-03"
        public int Visits { get; set; }
    }
}
