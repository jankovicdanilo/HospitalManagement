using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using System.Runtime.InteropServices;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
        {
            this.appointmentRepository = appointmentRepository;
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
        }

        public async Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(request.Id);

            if(appointmentDomain == null)
            {
                return Result<AppointmentUpdateResponseDto>.Fail($"Appointment with the id {request.Id} not found", "INVALID_ID");
            }

            var patientDomain = await patientRepository.GetByIdAsync(request.PatientId);

            if (patientDomain == null)
            {
                return Result<AppointmentUpdateResponseDto>.Fail($"Patient with the id {request.PatientId} not found", "INVALID_PATIENT_ID");
            }

            var doctorDomain = await doctorRepository.GetById(request.DoctorId);

            if(doctorDomain == null)
            {
                return Result<AppointmentUpdateResponseDto>.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            } 

            appointmentDomain.PatientId = request.PatientId;
            appointmentDomain.DoctorId = request.DoctorId;
            appointmentDomain.DateTime = request.DateTime;
            appointmentDomain.Status = request.Status;
            appointmentDomain.Notes = request.Notes;

            appointmentDomain = await appointmentRepository.UpdateAsync(appointmentDomain);

            var result = new AppointmentUpdateResponseDto
                (
                    appointmentDomain.Id,
                    appointmentDomain.PatientId,
                    appointmentDomain.DoctorId,
                    appointmentDomain.DateTime,
                    appointmentDomain.Status,
                    appointmentDomain.Notes
                );

            return Result<AppointmentUpdateResponseDto>.Ok(result);
        }
    }
}
