using AutoMapper;
using HospitalManagement.CommandService.Models.Doctor;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Models.DTOs.Doctor;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Events;
using MassTransit;
using MassTransit.Serialization;

namespace HospitalManagement.CommandService.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorService> logger;
        private readonly IPublishEndpoint publishEndpoint;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper,
            ILogger<DoctorService> logger, IPublishEndpoint publishEndpoint)
        {
            this.doctorRepository = doctorRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task<Result<DoctorResponseDto>> CreateAsync(DoctorCreateRequestDto request)
        {
            var doctorDomain = mapper.Map<Doctor>(request);
            doctorDomain = await doctorRepository.CreateAsync(doctorDomain);

            await publishEndpoint.Publish(new DoctorCreated(
                CorrelationId: Guid.NewGuid(),
                Id: doctorDomain!.Id,
                FirstName: doctorDomain.FirstName,
                LastName: doctorDomain.LastName,
                Specialization: doctorDomain.Specialization,
                Email: doctorDomain.Email,
                Phone: doctorDomain.Phone));

            logger.LogInformation("Doctor created with id {Id}, DoctorCreated event published", doctorDomain.Id);
            var result = mapper.Map<DoctorResponseDto>(doctorDomain);
            return Result<DoctorResponseDto>.Ok(result);
        }

        public async Task<Result<DoctorResponseDto>> UpdateAsync(DoctorUpdateRequestDto request)
        {
            var doctorDomain = await doctorRepository.GetByIdAsync(request.Id);
            if (doctorDomain is null)
            {
                logger.LogWarning("Doctor with id {Id} not found for update", request.Id);
                return Result<DoctorResponseDto>.Fail($"Doctor with the id {request.Id} doesn't exist!", "INVALID_ID", ErrorType.NotFound);
            }
            mapper.Map(request, doctorDomain);
            doctorDomain = await doctorRepository.UpdateAsync(doctorDomain);

            await publishEndpoint.Publish(new DoctorUpdated(
                CorrelationId: Guid.NewGuid(),
                Id:  doctorDomain.Id,
                FirstName: doctorDomain.FirstName,
                LastName: doctorDomain.LastName,
                Specialization: doctorDomain.Specialization,
                Email: doctorDomain.Email,
                Phone: doctorDomain.Phone));

            logger.LogInformation("Doctor updated with id {Id}, DoctorUpdated event published", doctorDomain.Id);
            var result = mapper.Map<DoctorResponseDto>(doctorDomain);
            return Result<DoctorResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var doctor = await doctorRepository.GetByIdAsync(id);
            if (doctor is null)
            {
                logger.LogWarning("Doctor with id {Id} not found for deletion", id);
                return Result.Fail($"Doctor with the id {id} does not exist", "INVALID_ID", ErrorType.NotFound);
            }
            await doctorRepository.Delete(id);

            await publishEndpoint.Publish(new DoctorDeleted(
                CorrelationId: Guid.NewGuid(),
                Id: id));

            logger.LogInformation("Doctor deleted with id {Id}, DoctorDeleted event published", id);
            return Result.Ok("Doctor has been deleted!");
        }
    }
}