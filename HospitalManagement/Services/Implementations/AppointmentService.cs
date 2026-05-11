using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository apointmentRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;

        public AppointmentService(IAppointmentRepository apointmentRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
        {
            this.apointmentRepository = apointmentRepository;
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
        }

        public async Task<Result<CreateAppointmentResponseDto>> CreateAsync(CreateAppointmentRequestDto request)
        {
            var doctorDomain = await doctorRepository.GetById(request.DoctorId);

            if(doctorDomain == null)
            {
                return Result<CreateAppointmentResponseDto>.Fail($"Doctor with the id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            }

            var patientDomain = await patientRepository.GetByIdAsync(request.PatientId);

            if (patientDomain == null)
            {
                return Result<CreateAppointmentResponseDto>.Fail($"Patient with the id {request.PatientId} not found", "INVALID_PATIENT_ID");
            }

            if(request.DateTime <  DateTime.UtcNow)
            {
                return Result<CreateAppointmentResponseDto>.Fail($"Appointment can't be set before today", "INVALID_DATE_TIME");
            }

            var appointmentDomain = new Appointment
            {
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                DateTime = request.DateTime,
                Status = request.Status,
                Notes = request.Notes
            };

            appointmentDomain = await apointmentRepository.CreateAsync(appointmentDomain);

            var result = new CreateAppointmentResponseDto
                (
                    appointmentDomain.Id,
                    appointmentDomain.PatientId,
                    appointmentDomain.DoctorId,
                    appointmentDomain.DateTime,
                    appointmentDomain.Status,
                    appointmentDomain.Status
                );

            return Result<CreateAppointmentResponseDto>.Ok(result);
        }
    }
}
