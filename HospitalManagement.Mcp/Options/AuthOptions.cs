using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HospitalManagement.Mcp.Options
{
    public class AuthOptions
    {
        public const string SectionName = "Auth";

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
