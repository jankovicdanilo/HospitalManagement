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
        private readonly AppointmentSettings appointmentSettings;

        public AppointmentValidation(IDoctorRepository doctorRepository, IPatientRepository patientRepository, 
            IAppointmentRepository appointmentRepository, IOptions<AppointmentSettings> appointmentSettings)
        {
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
            this.appointmentRepository = appointmentRepository;
            this.appointmentSettings = appointmentSettings.Value;
        }

        private async Task<bool> CheckDoctorId(int id)
        {
            return await doctorRepository.GetByIdAsync(id) != null;
        }

        private async Task<bool> CheckPatientId(int id)
        {
            return await patientRepository.GetByIdAsync(id) != null;
        }

        private bool CheckDate(DateTime dateTime)
        {
            return dateTime <= DateTime.UtcNow;
        }

        private bool IsWithinWorkingHours(DateTime dateTime)
        {
            var totalMinutes = dateTime.Hour * 60 + dateTime.Minute;
            var startMinutes = appointmentSettings.WorkStartHour * 60;
            var endMinutes = appointmentSettings.WorkEndHour * 60 - 30;

            return totalMinutes >= startMinutes && totalMinutes <= endMinutes;
        }

        private async Task<bool> CheckDoctorAvailability(int doctorId, DateTime dateTime, TimeSpan duration, int? excludeAppointmentId = null)
        {
            var appointments = await appointmentRepository.GetByDoctorIdAsync(doctorId);

            return !appointments.Any(a => a.Id != excludeAppointmentId && dateTime < a.DateTime.Add(a.Duration) && 
                                    dateTime.Add(duration) > a.DateTime);
        }

        public async Task<Result> ValidateAll(AppointmentCreateRequestDto request)
        {
            if (!await CheckDoctorId(request.DoctorId))
            {
                return Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            }

            if (!await CheckPatientId(request.PatientId))
            {
                return Result.Fail($"Patient with the id {request.PatientId} not found", "INVALID_PATIENT_ID");
            }

            if (CheckDate(request.DateTime))
            {
                return Result.Fail($"Appointment can't be set before today", "INVALID_DATE_TIME");
            }

            if (!IsWithinWorkingHours(request.DateTime))
            {
                return Result.Fail($"Appointment can't be set outside working hours", "INVALID_DATE_TIME");
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
            if (!await CheckDoctorId(request.DoctorId))
            {
                return Result.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            }

            if (!await CheckPatientId(request.PatientId))
            {
                return Result.Fail($"Patient with the id {request.PatientId} not found", "INVALID_PATIENT_ID");
            }

            if (CheckDate(request.DateTime))
            {
                return Result.Fail($"Appointment can't be set before today", "INVALID_DATE_TIME");
            }

            if (!IsWithinWorkingHours(request.DateTime))
            {
                return Result.Fail($"Appointment can't be set outside working hours", "INVALID_DATE_TIME");
            }

            if (!await CheckDoctorAvailability(request.DoctorId, request.DateTime, request.Duration, request.Id))
            {
                return Result.Fail($"Doctor with the id {request.DoctorId} is not available at that time", "DOCTOR_NOT_AVAILABLE");
            }

            return Result.Ok("Validation ok");
        }
    }
}
