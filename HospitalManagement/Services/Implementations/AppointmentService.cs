using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            this.appointmentRepository = appointmentRepository;
        }

        public async Task<Result<List<AppointmentListResponseDto>>> GetAllAsync()
        {
            var appointments = await appointmentRepository.GetAllAsync();

            var result = new List<AppointmentListResponseDto>();

            foreach(var appointment in appointments)
            {
                result.Add(new AppointmentListResponseDto
                    (
                        appointment.Id,
                        appointment.PatientId,
                        appointment.DoctorId,
                        appointment.DateTime,
                        appointment.Status,
                        appointment.Notes
                    ));
            }

            return Result<List<AppointmentListResponseDto>>.Ok(result);
        }

        public async Task<Result<AppointmentResponseDto>> GetByIdAsync(int id)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(id);

            if(appointmentDomain == null)
            {
                return Result<AppointmentResponseDto>.Fail($"Appointment with the id {id} not found", "INVALID_ID");
            }

            var result = new AppointmentResponseDto
                (
                    appointmentDomain.Id,
                    appointmentDomain.PatientId,
                    appointmentDomain.DoctorId,
                    appointmentDomain.DateTime,
                    appointmentDomain.Status,
                    appointmentDomain.Notes
                );

            return Result<AppointmentResponseDto>.Ok(result);
        }
    }
}
