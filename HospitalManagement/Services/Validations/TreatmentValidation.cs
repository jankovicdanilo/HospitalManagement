using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Treatment;
using HospitalManagement.Models.Enums;
using HospitalManagement.Repositories.Interfaces;

namespace HospitalManagement.Services.Validations
{
    public class TreatmentValidation
    {
        private readonly ITreatmentRepository treatmentRepository;
        private readonly IAppointmentRepository appointmentRepository;

        public TreatmentValidation(ITreatmentRepository treatmentRepository, IAppointmentRepository appointmentRepository)
        {
            this.treatmentRepository = treatmentRepository;
            this.appointmentRepository = appointmentRepository;
        }

        private async Task<bool> CheckTreatmentAlreadyExist(int id)
        {
            return await treatmentRepository.TreatmentExists(id);
        }

        private async Task<bool> IsAppointmentCompleted(AppointmentStatus status)
        {
            return status == AppointmentStatus.Completed;
        }

        public async Task<Result> ValidateAll(TreatmentCreateRequestDto request)
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);

            if (appointment == null)
            {
                return Result.Fail(
                    $"Appointment with id {request.AppointmentId} not found",
                    "APPOINTMENT_NOT_FOUND");
            }

            if (await CheckTreatmentAlreadyExist(request.AppointmentId))
            {
                return Result.Fail($"Treatment for appointment " +
                    $"with the id {request.AppointmentId}" +
                    $" already exists", "TREATMENT_EXISTS");
            }

            if(!await IsAppointmentCompleted(appointment.Status))
            {
                return Result.Fail(
                    "Treatment can only be added to a completed appointment",
                    "APPOINTMENT_NOT_COMPLETED");
            }

            return Result.Ok("Validation ok");
        }
    }
}
