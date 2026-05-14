using Azure.Core;
using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Validations
{
    public class AppointmentValidation
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;

        public AppointmentValidation(IDoctorRepository doctorRepository, IPatientRepository patientRepository)
        {
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
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

        public async Task<Result> ValidateAll(int doctorId,
            int patientId, DateTime dateTime)
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

            return Result.Ok("Validation ok");
        }
    }
}
