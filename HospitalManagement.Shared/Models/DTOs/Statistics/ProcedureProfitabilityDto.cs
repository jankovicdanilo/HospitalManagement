using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Models.DTOs.Statistics
{
    public class ProcedureProfitabilityDto
    {
        public int ProcedureId { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public int TimesPerformed { get; set; }
        public decimal Revenue { get; set; }
    }
}
