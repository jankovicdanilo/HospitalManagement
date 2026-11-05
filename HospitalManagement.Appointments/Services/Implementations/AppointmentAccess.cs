using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Security;
using System.Security.Claims;

namespace HospitalManagement.Appointments.Services.Implementations
{
    public class AppointmentAccess : IAppointmentAccess
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly ILogger<AppointmentAccess> logger;

        public AppointmentAccess(IAppointmentRepository appointmentRepository, ILogger<AppointmentAccess> logger)
        {
            this.appointmentRepository = appointmentRepository;
            this.logger = logger;
        }

        public Result<int?> GetDoctorScope(ClaimsPrincipal user)
        {
            if(user.IsInRole(RoleNames.Admin) || user.IsInRole(RoleNames.Mcp))
            {
                return Result<int?>.Ok(null);
            }

            if (!user.IsDoctor())
            {
                logger.LogWarning("User {User} with role {Role} has no access to appointments",
                    user.Identity?.Name, user.FindFirst(ClaimTypes.Role)?.Value);
                return Result<int?>.Fail("You do not have access to appointments", "FORBIDDEN", ErrorType.Forbidden);
            }

            var doctorId = user.GetDoctorId();
            if(doctorId == null)
            {
                logger.LogWarning("Doctor {User} has no doctorId in the token, access refused", user.Identity?.Name);
                return Result<int?>.Fail("Your account is not linked to a doctor record",
                    "DOCTOR_NOT_LINKED", ErrorType.Forbidden);
            }

            return Result<int?>.Ok(doctorId);
        }

        public Result CheckDoctor(ClaimsPrincipal user, int doctorId)
        {
            var scope = GetDoctorScope(user);
            if (!scope.Success)
            {
                return Result.Fail(scope.Message, scope.ErrorCode, scope.ErrorType);
            }

            if(scope.Data == null || scope.Data == doctorId)
            {
                return Result.Ok();
            }

            logger.LogWarning("Doctor {OwnDoctorId} tried to act for doctor {DoctorId}", scope.Data, doctorId);

            return Result.Fail("You can only work with your own appointments", "NOT_YOUR_APPOINTMENT", ErrorType.Forbidden);
        }

        public async Task<Result> CheckAppointmentAsync(ClaimsPrincipal user, int appointmentId)
        {
            var scope = GetDoctorScope(user);
            if (!scope.Success)
            {
                return Result.Fail(scope.Message, scope.ErrorCode, scope.ErrorType);
            }

            if(scope.Data == null)
            {
                return Result.Ok();
            }

            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);
            if(appointment == null)
            {
                return Result.Ok();
            }

            var checkDoctor = CheckDoctor(user, appointment.DoctorId);
            if (checkDoctor.Success)
            {
                logger.LogInformation("Doctor {DoctorId} accessed own appointment {AppointmentId}", scope.Data, appointmentId);
            }

            return checkDoctor;
        }

        public async Task<Result> CheckPatientAsync(ClaimsPrincipal user, int patientId)
        {
            var scope = GetDoctorScope(user);
            if (!scope.Success)
            {
                return Result.Fail(scope.Message, scope.ErrorCode, scope.ErrorType);
            }

            if(scope.Data == null)
            {
                return Result.Ok();
            }

            var appointments = await appointmentRepository.GetByPatientIdAsync(patientId);
            if(!appointments.Any(a => a.DoctorId == scope.Data))
            {
                logger.LogWarning("Doctor {DoctorId} tried to open patient {PatientId} who is not theirs", scope.Data, patientId);
                return Result.Fail("You can only see your own patients", "NOT_YOUR_PATIENT", ErrorType.Forbidden);
            }

            logger.LogInformation("Doctor {DoctorId} accessed own patient {PatientId}", scope.Data, patientId);

            return Result.Ok();
        }
    }
}
