using AutoMapper;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Models.Patient;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Events;
using MassTransit;

namespace HospitalManagement.CommandService.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;
        private readonly ILogger<PatientService> logger;
        private readonly IPublishEndpoint publishEndpoint;

        public PatientService(IPatientRepository patientRepository, IMapper mapper, 
            ILogger<PatientService> logger, IPublishEndpoint publishEndpoint)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task<Result<PatientCreateResponseDto?>> CreateAsync(PatientCreateRequestDto request)
        {
            var patientExists = await patientRepository.GetByEmailAsync(request.Email);
            if (patientExists != null)
            {
                logger.LogWarning("Patient creation failed, email {Email} already exists", request.Email);
                return Result<PatientCreateResponseDto?>.Fail($"Email {request.Email} already exists", "INVALID_EMAIL");
            }
            var patientDomain = mapper.Map<Patient>(request);
            patientDomain = await patientRepository.CreateAsync(patientDomain);

            await publishEndpoint.Publish(new PatientCreated
            (
                CorrelationId: Guid.NewGuid(),
                Id: patientDomain.Id,
                FirstName: patientDomain.Name,
                LastName: patientDomain.LastName,
                Email: patientDomain.Email,
                Phone: patientDomain.Phone,
                DateOfBirth: patientDomain.DateOfBirth
            ));

            logger.LogInformation("Patient created with id {Id}, PatientCreated event published", patientDomain.Id);
            var result = mapper.Map<PatientCreateResponseDto>(patientDomain);
            return Result<PatientCreateResponseDto?>.Ok(result);
        }

        public async Task<Result<PatientUpdateResponseDto>> UpdateAsync(PatientUpdateRequestDto request)
        {
            var patientDomain = await patientRepository.GetByIdAsync(request.Id);
            if (patientDomain == null)
            {
                logger.LogWarning("Patient with id {Id} not found for update", request.Id);
                return Result<PatientUpdateResponseDto>.Fail($"Patient with the id {request.Id} not found", "INVALID_ID");
            }
            if (await patientRepository.EmailExists(request.Email) && request.Email != patientDomain.Email)
            {
                logger.LogWarning("Patient update failed, email {Email} already exists", request.Email);
                return Result<PatientUpdateResponseDto>.Fail($"Email {request.Email} already exists", "INVALID_EMAIL");
            }
            mapper.Map(request, patientDomain);
            await patientRepository.UpdateAsync(patientDomain);

            await publishEndpoint.Publish(new PatientUpdated
            (
                CorrelationId: Guid.NewGuid(),
                Id: patientDomain.Id,
                FirstName: patientDomain.Name,
                LastName: patientDomain.LastName,
                Email: patientDomain.Email,
                Phone: patientDomain.Phone
            ));

            logger.LogInformation("Patient with id {Id} updated, PatientUpdated event published", patientDomain.Id);
            var result = mapper.Map<PatientUpdateResponseDto>(patientDomain);
            return Result<PatientUpdateResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var patientDomain = await patientRepository.Delete(id);
            if (patientDomain == null)
            {
                logger.LogWarning("Patient with id {Id} not found for deletion", id);
                return Result.Fail($"Patient with the id {id} not found", "INVALID_ID");
            }

            await publishEndpoint.Publish(new PatientDeleted
            (
                CorrelationId: Guid.NewGuid(),
                Id: id
            ));

            logger.LogInformation("Patient with id {Id} deleted, PatientDeleted event published", id);
            return Result.Ok($"Patient with id {id} deleted");
        }
    }
}