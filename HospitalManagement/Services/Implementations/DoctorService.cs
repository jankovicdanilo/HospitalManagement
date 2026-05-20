using AutoMapper;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Doctor;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

namespace HospitalManagement.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IMapper mapper;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper)
        {
            this.doctorRepository = doctorRepository;
            this.mapper = mapper;
        }

        public async Task<Result<DoctorResponseDto>> CreateAsync(DoctorCreateRequestDto request)
        {
            var doctorDomain = mapper.Map<Doctor>(request);

            doctorDomain = await doctorRepository.CreateAsync(doctorDomain);

            var result = mapper.Map<DoctorResponseDto>(doctorDomain);

            return Result<DoctorResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var doctor = await doctorRepository.GetByIdAsync(id);

            if(doctor is null)
            {
                return Result.Fail($"Doctor with the id {id} does not exist", "INVALID_ID", ErrorType.NotFound);
            }

            await doctorRepository.Delete(id);

            return Result.Ok("Doctor has been deleted!");
        }

        public async Task<Result<List<DoctorResponseDto>>> GetAllAsync()
        {
            var doctors = await doctorRepository.GetAllAsync();

            var result = mapper.Map<List<DoctorResponseDto>>(doctors);

            return Result<List<DoctorResponseDto>>.Ok(result);
        }

        public async Task<Result<DoctorResponseDto>> GetByIdAsync(int id)
        {
            var doctor = await doctorRepository.GetByIdAsync(id);

            if(doctor is null)
            {
                return Result<DoctorResponseDto>.Fail($"Doctor with the id {id} was not found", "INVALID_ID",
                    ErrorType.NotFound);
            }

            var result = mapper.Map<DoctorResponseDto>(doctor);

            return Result<DoctorResponseDto>.Ok(result);
        }

        public async Task<Result<DoctorResponseDto>> UpdateAsync(DoctorUpdateRequestDto request)
        {
            var doctorDomain = await doctorRepository.GetByIdAsync(request.Id);

            if(doctorDomain is null)
            {
                return Result<DoctorResponseDto>.Fail
                        ($"Doctor with the id {request.Id} doesn't exist!","INVALID_ID", ErrorType.NotFound);
            }

            mapper.Map(request, doctorDomain);

            doctorDomain = await doctorRepository.UpdateAsync(doctorDomain);

            var result = mapper.Map<DoctorResponseDto>(doctorDomain);

            return Result<DoctorResponseDto>.Ok(result);
        }
    }
}
