using HospitalManagement.Common;

namespace HospitalManagement.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Result> Delete(int id);
    }
}
