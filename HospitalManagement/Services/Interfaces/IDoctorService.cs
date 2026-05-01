using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Doctor;

namespace HospitalManagement.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<DoctorResponseDto>> Create(DoctorCreateRequestDto request);

        Task<Result<List<DoctorResponseDto>>> GetAll();

        Task<Result<DoctorResponseDto>> GetById(int id);

        Task<Result<DoctorResponseDto>> Update(DoctorUpdateRequestDto request);

        Task<Result> Delete(int id);
    }
}
