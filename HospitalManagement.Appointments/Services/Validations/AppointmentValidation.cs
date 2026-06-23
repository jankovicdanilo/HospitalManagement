using HospitalManagement.Shared.Common;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Clients.Interfaces;

namespace HospitalManagement.Appointments.Services.Validations
{
    public class AppointmentValidation : IAppointmentValidation
    {
        private readonly IHospitalManagementClient hospitalClient;
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentValidation(IAppointmentRepository appointmentRepository,
            IHospitalManagementClient hospitalClient)
        {
            this.appointmentRepository = appointmentRepository;
            this.hospitalClient = hospitalClient;
        }

        public async Task<Result> ValidateAll(AppointmentCreateRequestDto request)
        {
            var doctor = await hospitalClient.GetDoctorAsync(request.DoctorId);
            if(doctor == null)
            {
                return Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            }

            var patient = await hospitalClient.GetPatientAsync(request.PatientId);
            if (patient == null)
            {
                return Result.Fail($"Patient with the id {request.PatientId} not found", "INVALID_PATIENT_ID");
            }
               

            var schedule = await hospitalClient.GetDoctorScheduleAsync(request.DoctorId, request.DateTime.DayOfWeek);

            if (schedule == null)
            {
                return Result.Fail($"Doctor does not work on " +
                    $"{request.DateTime.ToString("dddd, dd MMM yyyy", 
                    System.Globalization.CultureInfo.InvariantCulture)}", "DOCTOR_NOT_AVAILABLE");
            }
                
            if (request.DateTime.Hour < schedule.StartHour || request.DateTime.Hour * 60 + request.DateTime.Minute +
                (int)request.Duration.TotalMinutes > schedule.EndHour * 60)
                return Result.Fail($"Doctor works {schedule.StartHour}:00 - {schedule.EndHour}:00", "OUTSIDE_WORKING_HOURS");

            if (!await CheckDoctorAvailability(request.DoctorId, request.DateTime, request.Duration))
                return Result.Fail($"Doctor with the id {request.DoctorId} is not available at that time", "DOCTOR_SLOT_TAKEN");

            return Result.Ok("Validation ok");
        }

        public async Task<Result> ValidateAll(AppointmentUpdateRequestDto request)
        {
            var doctor = await hospitalClient.GetDoctorAsync(request.DoctorId);
            if (doctor == null)
                return Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID");

            var patient = await hospitalClient.GetPatientAsync(request.PatientId);
            if (patient == null)
                return Result.Fail($"Patient with the id {request.PatientId} not found", "INVALID_PATIENT_ID");

            var schedule = await hospitalClient.GetDoctorScheduleAsync(request.DoctorId, request.DateTime.DayOfWeek);
            if (schedule == null)
                return Result.Fail($"Doctor does not work on " +
                    $"{request.DateTime.ToString("dddd, dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture)}", "DOCTOR_NOT_AVAILABLE");

            if (request.DateTime.Hour < schedule.StartHour || request.DateTime.Hour * 60 + request.DateTime.Minute
                + (int)request.Duration.TotalMinutes > schedule.EndHour * 60)
                return Result.Fail($"Doctor works {schedule.StartHour}:00 - {schedule.EndHour}:00", "OUTSIDE_WORKING_HOURS");

            if (!await CheckDoctorAvailability(request.DoctorId, request.DateTime, request.Duration, request.Id))
                return Result.Fail($"Doctor with the id {request.DoctorId} is not available at that time", "DOCTOR_SLOT_TAKEN");

            return Result.Ok("Validation ok");
        }

        private async Task<bool> CheckDoctorAvailability(int doctorId, DateTime dateTime, TimeSpan duration, int? excludeAppointmentId = null)
        {
            var appointments = await appointmentRepository.GetByDoctorIdAsync(doctorId);

            return !appointments.Any(a => a.Id != excludeAppointmentId &&
                                    dateTime < a.DateTime.Add(a.Duration) &&
                                    dateTime.Add(duration) > a.DateTime);
        }
    }
}