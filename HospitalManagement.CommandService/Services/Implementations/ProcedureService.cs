using AutoMapper;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Models.Procedure;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Events;
using MassTransit;

namespace HospitalManagement.CommandService.Services.Implementations
{
    public class ProcedureService : IProcedureService
    {
        private readonly IProcedureRepository procedureRepository;
        private readonly IMapper mapper;
        private readonly ILogger<ProcedureService> logger;
        private readonly IPublishEndpoint publishEndpoint;

        public ProcedureService(IProcedureRepository procedureRepository, IMapper mapper, 
            ILogger<ProcedureService> logger, IPublishEndpoint publishEndpoint)
        {
            this.procedureRepository = procedureRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task<Result<ProcedureCreateResponseDto>> CreateAsync(ProcedureCreateRequestDto request)
        {
            var procedureDomain = mapper.Map<Procedure>(request);
            procedureDomain = await procedureRepository.CreateAsync(procedureDomain);

            await publishEndpoint.Publish(new ProcedureCreated
            (
                CorrelationId: Guid.NewGuid(),
                Id: procedureDomain.Id,
                Name: procedureDomain.Name,
                Price: procedureDomain.Price
            ));

            logger.LogInformation("Procedure created with id {id}, ProcedureCreated event published", procedureDomain.Id);
            var result = mapper.Map<ProcedureCreateResponseDto>(procedureDomain);
            return Result<ProcedureCreateResponseDto>.Ok(result);
        }

        public async Task<Result<ProcedureUpdateResponseDto>> UpdateAsync(int id, ProcedureUpdateRequestDto request)
        {
            var procedureDomain = mapper.Map<Procedure>(request);
            procedureDomain = await procedureRepository.UpdateAsync(id, procedureDomain);
            if (procedureDomain == null)
            {
                logger.LogWarning("Procedure with id {id} not found", id);
                return Result<ProcedureUpdateResponseDto>.Fail($"Procedure with id {id} not found", "INVALID_ID");
            }

            await publishEndpoint.Publish(new ProcedureUpdated
            (
                CorrelationId: Guid.NewGuid(),
                Id: procedureDomain.Id,
                Name: procedureDomain.Name,
                Price: procedureDomain.Price
            ));

            logger.LogInformation("Procedure with id {id} updated, ProcedureUpdated event published", procedureDomain.Id);
            var result = mapper.Map<ProcedureUpdateResponseDto>(procedureDomain);
            return Result<ProcedureUpdateResponseDto>.Ok(result);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var procedureDomain = await procedureRepository.DeleteAsync(id);
            if (procedureDomain == null)
            {
                logger.LogWarning("Procedure with id {id} not found", id);
                return Result.Fail($"Procedure with id {id} not found", "INVALID_ID");
            }

            await publishEndpoint.Publish(new ProcedureDeleted
            (
                CorrelationId: Guid.NewGuid(),
                Id: id
            ));

            logger.LogInformation("Procedure with id {id} deleted, ProcedureDeleted event published", id);
            return Result.Ok("Procedure deleted");
        }
    }
}