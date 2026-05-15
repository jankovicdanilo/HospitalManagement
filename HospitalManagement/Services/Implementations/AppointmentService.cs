using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Implementations;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Services.Validations;
using Microsoft.AspNetCore.Http.Features;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly AppointmentUpdateValidation appointmentValidation;

        public AppointmentService(IAppointmentRepository apointmentRepository, AppointmentUpdateValidation appointmentValidation)
        {
            this.appointmentRepository = apointmentRepository;
            this.appointmentValidation = appointmentValidation;
        }

        public async Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request)
        {
            var validatedAppointment = await appointmentValidation.ValidateAll(request.Id, request.DoctorId, request.PatientId, request.DateTime,
                request.Duration);

            if (!validatedAppointment.Success)
            {
                return Result<AppointmentUpdateResponseDto>.Fail(validatedAppointment.Message, validatedAppointment.ErrorCode);
            }

            var appointmentDomain = validatedAppointment.Data;

            appointmentDomain.PatientId = request.PatientId;
            appointmentDomain.DoctorId = request.DoctorId;
            appointmentDomain.DateTime = request.DateTime;
            appointmentDomain.Duration = request.Duration;
            appointmentDomain.Status = request.Status;
            appointmentDomain.Notes = request.Notes;

            appointmentDomain = await appointmentRepository.UpdateAsync(appointmentDomain);

            var result = new AppointmentUpdateResponseDto
                (
                    appointmentDomain.Id,
                    appointmentDomain.PatientId,
                    appointmentDomain.DoctorId,
                    appointmentDomain.DateTime,
                    appointmentDomain.Duration,
                    appointmentDomain.Status,
                    appointmentDomain.Notes
                );

            return Result<AppointmentUpdateResponseDto>.Ok(result);
        }
    }
}
