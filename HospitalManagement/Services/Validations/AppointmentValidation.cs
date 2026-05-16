using Azure.Core;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Validations
{
    public class AppointmentValidation
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentValidation(IDoctorRepository doctorRepository, IPatientRepository patientRepository, IAppointmentRepository appointmentRepository)
        {
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
            this.appointmentRepository = appointmentRepository;
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

        private async Task<bool> CheckDoctorAvailability(int doctorId, DateTime dateTime, TimeSpan duration, int? excludeAppointmentId = null)
        {
            var appointments = await appointmentRepository.GetByDoctorIdAsync(doctorId);

            return !appointments.Any(a => a.Id != excludeAppointmentId && dateTime < a.DateTime.Add(a.Duration) && dateTime.Add(duration) > a.DateTime);
        }

        public async Task<Result> ValidateAll(int doctorId,
            int patientId, DateTime dateTime, TimeSpan duration, int? appointmentId = null)
        {
            if (!await CheckDoctorId(doctorId))
            {
                return Result.Fail($"Doctor with the id {doctorId} not found", "INVALID_DOCTOR_ID");
            }

            if (!await CheckPatientId(patientId))
            {
                return Result.Fail($"Patient with the id {patientId} not found", "INVALID_PATIENT_ID");
            }

            if (CheckDate(dateTime))
            {
                return Result.Fail($"Appointment can't be set before today", "INVALID_DATE_TIME");
            }
            
            if (!await CheckDoctorAvailability(doctorId, dateTime, duration, appointmentId))
            {
                return Result.Fail($"Doctor with the id {doctorId} is not available at that time", "DOCTOR_NOT_AVAILABLE");
            }

            return Result.Ok("Validation ok");
        }
    }
}
