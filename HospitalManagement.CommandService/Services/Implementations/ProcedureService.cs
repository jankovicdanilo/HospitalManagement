using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.CommandService.Services.Implementations
{
    public class ProcedureService : IProcedureService
    {
        private readonly IProcedureRepository procedureRepository;
        private readonly IMapper mapper;
        private readonly ILogger<ProcedureService> logger;

        public ProcedureService(IProcedureRepository procedureRepository, IMapper mapper, 
            ILogger<ProcedureService> logger)
        {
            this.procedureRepository = procedureRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Result<ProcedureCreateResponseDto>> CreateAsync(ProcedureCreateRequestDto request)
        {
            var procedureDomain = mapper.Map<Procedure>(request);
            procedureDomain = await procedureRepository.CreateAsync(procedureDomain);

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
                return Result<ProcedureUpdateResponseDto>.Fail($"Procedure with id {id} not found", "INVALID_ID", 
                    ErrorType.NotFound);
            }

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
                return Result.Fail($"Procedure with id {id} not found", "INVALID_ID", ErrorType.NotFound);
            }

            logger.LogInformation("Procedure with id {id} deleted, ProcedureDeleted event published", id);
            return Result.Ok("Procedure deleted");
        }
    }
}