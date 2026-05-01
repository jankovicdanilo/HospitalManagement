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

        public DoctorService(IDoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
        }

        public async Task<Result<DoctorResponseDto>> Create(DoctorCreateRequestDto request)
        {
            var doctorDomain = new Doctor
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Specialization = request.Specialization,
                Phone = request.Phone
            };

            doctorDomain = await doctorRepository.Create(doctorDomain);

            var result = new DoctorResponseDto
            (
                doctorDomain.Id,
                doctorDomain.FirstName,
                doctorDomain.LastName,
                doctorDomain.Specialization,
                doctorDomain.Email,
                doctorDomain.Phone
            );

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

            var result = doctors.Select(doctor => new DoctorResponseDto
            (
                doctor.Id,
                doctor.FirstName,
                doctor.LastName,
                doctor.Specialization,
                doctor.Email,
                doctor.Phone
            )).ToList();

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

            var result = new DoctorResponseDto
            (
                doctor.Id,
                doctor.FirstName,
                doctor.LastName,
                doctor.Specialization,
                doctor.Email,
                doctor.Phone
            );

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

            var result = new DoctorResponseDto
            (
                doctor.Id,
                doctor.FirstName,
                doctor.LastName,
                doctor.Specialization,
                doctor.Email,
                doctor.Phone
            );

            return Result<DoctorResponseDto>.Ok(result);
        }
    }
}
