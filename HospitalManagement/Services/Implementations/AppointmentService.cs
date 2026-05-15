using AutoMapper;
using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Implementations;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Services.Validations;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly AppointmentUpdateValidation appointmentValidation;
        private readonly AppointmentSettings appointmentSettings;
        private readonly IMapper mapper;

        public AppointmentService(IAppointmentRepository apointmentRepository, AppointmentUpdateValidation appointmentValidation,
            IMapper mapper, IOptions<AppointmentSettings> appointmentSettings)
        {
            this.appointmentRepository = apointmentRepository;
            this.appointmentValidation = appointmentValidation;
            this.mapper = mapper;
            this.appointmentSettings = appointmentSettings.Value;
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

            var result = mapper.Map<AppointmentUpdateResponseDto>(appointmentDomain);

            return Result<AppointmentUpdateResponseDto>.Ok(result);
        }

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            this.appointmentRepository = appointmentRepository;
        }

        public async Task<Result<List<AppointmentListResponseDto>>> GetAllAsync()
        {
            var appointments = await appointmentRepository.GetAllAsync();

            var result = mapper.Map<List<AppointmentListResponseDto>>(appointments);

            return Result<List<AppointmentListResponseDto>>.Ok(result);
        }

        public async Task<Result<AppointmentResponseDto>> GetByIdAsync(int id)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(id);

            if(appointmentDomain == null)
            {
                return Result<AppointmentResponseDto>.Fail($"Appointment with the id {id} not found", "INVALID_ID");
            }

            var result = mapper.Map<AppointmentResponseDto>(appointmentDomain);

            return Result<AppointmentResponseDto>.Ok(result);
        }

        public async Task<Result<List<TimeSlotDto>>> GetFreeSlotsAsync(int doctorId, DateOnly date)
        {
            var workStart = new TimeSpan(appointmentSettings.WorkStartHour ,0 , 0);
            var workEnd = new TimeSpan(appointmentSettings.WorkEndHour ,0 , 0);
            var slotSize = new TimeSpan(0, appointmentSettings.SlotSizeMinutes, 0);

            var appointments = await appointmentRepository.GetByDoctorIdAndDateAsync(doctorId, date);

            var freeSlots = new List<TimeSlotDto>();
            var current = workStart;

            while(current + slotSize <= workEnd)
            {
                var slotStart = date.ToDateTime(TimeOnly.FromTimeSpan(current));
                var slotEnd = slotStart.Add(slotSize);

                var isBooked = appointments.Any(a =>
                                                slotStart < a.DateTime.Add(a.Duration) &&
                                                slotEnd > a.DateTime);

                if (!isBooked)
                {
                    freeSlots.Add(new TimeSlotDto 
                    { 
                        Start = TimeOnly.FromDateTime(slotStart), 
                        End = TimeOnly.FromDateTime(slotEnd) 
                    });
                }
                                                
                current = current.Add(slotSize);
            }

            return Result<List<TimeSlotDto>>.Ok(freeSlots);
        }
    }
}
