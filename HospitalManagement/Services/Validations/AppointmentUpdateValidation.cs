using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Repositories.Interfaces;

namespace HospitalManagement.Services.Validations
{
    public class AppointmentUpdateValidation
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentUpdateValidation(IDoctorRepository doctorRepository, IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository)
        {
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
            this.appointmentRepository = appointmentRepository;
        }

        private async Task<bool> CheckDoctorId(int id)
        {
            return await doctorRepository.GetById(id) != null;
        }

        private async Task<bool> CheckPatientId(int id)
        {
            return await patientRepository.GetByIdAsync(id) != null;
        }

        private bool CheckDate(DateTime dateTime)
        {
            return dateTime <= DateTime.UtcNow;
        }

        private async Task<bool> CheckDoctorAvailability(int doctorId, DateTime dateTime, TimeSpan duration, int? excludeAppointmenId = null)
        {
            var appointments = await appointmentRepository.GetByDoctorIdAsync(doctorId);
            return !appointments.Any(a => a.Id != excludeAppointmenId && dateTime < a.DateTime.Add(a.Duration) && dateTime.Add(duration) > a.DateTime);
        }

        public async Task<Result<Appointment>> ValidateAll(int appointmentId, int doctorId,
            int patientId, DateTime dateTime, TimeSpan duration)
        {
            if (!await CheckDoctorId(doctorId))
            {
                return Result<Appointment>.Fail($"Doctor with the id {doctorId} not found", "INVALID_DOCTOR_ID");
            }

            if (!await CheckPatientId(patientId))
            {
                return Result<Appointment>.Fail($"Patient with the id {patientId} not found", "INVALID_PATIENT_ID");
            }

            if (CheckDate(dateTime))    
            {
                return Result<Appointment>.Fail($"Appointment can't be set before today", "INVALID_DATE_TIME");
            }

            var appointmentDomain = await appointmentRepository.GetByIdAsync(appointmentId);

            if(appointmentDomain == null)
            {
                return Result<Appointment>.Fail($"Appointment with the id {appointmentId} not found", "INVALID_APPOINTMENT_ID");
            }

            if(!await CheckDoctorAvailability(doctorId, dateTime, duration, appointmentId))
            {
                return Result<Appointment>.Fail($"Doctor with id {doctorId} is not available at that time", "DOCTOR_NOT_AVAILABLE");
            }

            return Result<Appointment>.Ok(appointmentDomain);
        }
    }
}
