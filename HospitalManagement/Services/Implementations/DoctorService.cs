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

        public async Task<Result<DoctorResponseDto>> Create(DoctorCreateRequestDto request)
        {
            var doctorDomain = mapper.Map<Doctor>(request);

            doctorDomain = await doctorRepository.Create(doctorDomain);

            var result = mapper.Map<DoctorResponseDto>(doctorDomain);

            return Result<DoctorResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var doctor = await doctorRepository.GetById(id);

            if(doctor is null)
            {
                return Result.Fail($"Doctor with the id {id} does not exist", "INVALID_ID", ErrorType.NotFound);
            }

            await doctorRepository.Delete(id);

            return Result.Ok("Doctor has been deleted!");
        }

        public async Task<Result<List<DoctorResponseDto>>> GetAll()
        {
            var doctors = await doctorRepository.GetAll();

            var result = mapper.Map<List<DoctorResponseDto>>(doctors);

            return Result<List<DoctorResponseDto>>.Ok(result);
        }

        public async Task<Result<DoctorResponseDto>> GetById(int id)
        {
            var doctor = await doctorRepository.GetById(id);

            if(doctor is null)
            {
                return Result<DoctorResponseDto>.Fail($"Doctor with the id {id} was not found", "INVALID_ID",
                    ErrorType.NotFound);
            }

            var result = mapper.Map<DoctorResponseDto>(doctor);

            return Result<DoctorResponseDto>.Ok(result);
        }

        public async Task<Result<DoctorResponseDto>> Update(DoctorUpdateRequestDto request)
        {
            var doctor = await doctorRepository.GetById(request.Id);

            if(doctor is null)
            {
                return Result<DoctorResponseDto>.Fail
                        ($"Doctor with the id {request.Id} doesn't exist!","INVALID_ID", ErrorType.NotFound);
            }

            doctor.FirstName = request.FirstName;
            doctor.LastName = request.LastName;
            doctor.Email = request.Email;
            doctor.Phone = request.Phone;
            doctor.Specialization = request.Specialization;

            doctor = await doctorRepository.Update(doctor);

            var result = mapper.Map<DoctorResponseDto>(doctor);

            return Result<DoctorResponseDto>.Ok(result);
        }
    }
}
