using HospitalManagement.Common;

namespace HospitalManagement.Services.Validations
{
    public interface IAppointmentProcedureValidation
    {
        Task<Result> ValidateForCreate(int appointmentId, int procedureId);
        Task<Result> ValidateForGet(int appointmentId, int procedureId);
        Task<Result> ValidateForDelete(int appointmentId, int procedureId);
    }
}
