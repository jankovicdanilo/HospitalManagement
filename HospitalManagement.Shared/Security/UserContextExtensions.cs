using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Security
{
    public static class UserContextExtensions
    {
        public static int? GetDoctorId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(AppClaims.DoctorId)?.Value;
            return int.TryParse(value, out var doctorId) ? doctorId : null;
        }

        public static bool IsDoctor(this ClaimsPrincipal user) => user.IsInRole(RoleNames.Doctor);
    }
}
