using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Common
{
    public class InvoiceResult
    {
        public byte[]? FileBytes { get; set; }
        public string? PatientName { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ContentType { get; set; }
        public string? FileExtension { get; set; }
    }
}
