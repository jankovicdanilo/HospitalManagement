using Azure.Core;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Validations
{
    public class AppointmentValidation
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IDoctorScheduleRepository doctorScheduleRepository;
        private readonly AppointmentSettings appointmentSettings;

        public AppointmentValidation(IDoctorRepository doctorRepository, IPatientRepository patientRepository, 
            IAppointmentRepository appointmentRepository, IOptions<AppointmentSettings> appointmentSettings, IDoctorScheduleRepository doctorScheduleRepository)
        {
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
            this.appointmentRepository = appointmentRepository;
            this.appointmentSettings = appointmentSettings.Value;
            this.doctorScheduleRepository = doctorScheduleRepository;
        }

        private async Task<DoctorSchedule?> GetDoctorSchedule(int doctorId, DateTime dateTime)
        {
            return await doctorScheduleRepository.GetByDoctorIdAndDayAsync(doctorId, dateTime.DayOfWeek);
        }

        private async Task<bool> CheckDoctorId(int id)
        {
            return await doctorRepository.GetByIdAsync(id) != null;
        }

        private async Task<bool> CheckPatientId(int id)
        {
            return await patientRepository.GetByIdAsync(id) != null;
        }

        private async Task<bool> CheckDoctorAvailability(int doctorId, DateTime dateTime, TimeSpan duration, int? excludeAppointmentId = null)
        {
            var appointments = await appointmentRepository.GetByDoctorIdAsync(doctorId);

            return !appointments.Any(a => a.Id != excludeAppointmentId && dateTime < a.DateTime.Add(a.Duration) && 
                                    dateTime.Add(duration) > a.DateTime);
        }

        public async Task<Result> ValidateAll(AppointmentCreateRequestDto request)
        {
            var schedule = await GetDoctorSchedule(request.DoctorId, request.DateTime);

            if(schedule == null)
            {
                return Result.Fail($"Doctor does not work on " +
                    $"{request.DateTime.ToString("dddd, dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture)}", "DOCTOR_NOT_AVAILABLE");
            }

            if(request.DateTime.Hour < schedule.StartHour || request.DateTime.Hour + request.Duration.Hours > schedule.EndHour)
            {
                return Result.Fail($"Doctor works {schedule.StartHour}:00 - {schedule.EndHour}:00", "OUTSIDE_WORKING_HOURS");
            }

            if (!await CheckDoctorId(request.DoctorId))
            {
                return Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            }

            if (!await CheckPatientId(request.PatientId))
            {
                return Result.Fail($"Patient with the id {request.PatientId} not found", "INVALID_PATIENT_ID");
            }

            if (!await CheckDoctorAvailability(request.DoctorId, request.DateTime, request.Duration))
            {
                return Result.Fail($"Doctor with the id {request.DoctorId} is not available at that time", 
                    "DOCTOR_NOT_AVAILABLE");
            }

            return Result.Ok("Validation ok");
        }

        public async Task<Result> ValidateAll(AppointmentUpdateRequestDto request)
        {
            var schedule = await GetDoctorSchedule(request.DoctorId, request.DateTime);

            if(schedule == null)
            {
                return Result.Fail($"Doctor does not work on " +
                    $"{request.DateTime.ToString("dddd, dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture)}", "DOCTOR_NOT_AVAILABLE");
            }

            if(request.DateTime.Hour < schedule.StartHour || request.DateTime.Hour + request.Duration.Hours > schedule.EndHour)
            {
                return Result.Fail($"Doctor works {schedule.StartHour}:00 - {schedule.EndHour}:00", "OUTSIDE_WORKING_HOURS");
            }

            if (!await CheckDoctorId(request.DoctorId))
            {
                return Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            }

            if (!await CheckPatientId(request.PatientId))
            {
                return Result.Fail($"Patient with the id {request.PatientId} not found", "INVALID_PATIENT_ID");
            }

            if (!await CheckDoctorAvailability(request.DoctorId, request.DateTime, request.Duration, request.Id))
            {
                return Result.Fail($"Doctor with the id {request.DoctorId} is not available at that time", "DOCTOR_NOT_AVAILABLE");
            }

            return Result.Ok("Validation ok");
        }
    }
}
