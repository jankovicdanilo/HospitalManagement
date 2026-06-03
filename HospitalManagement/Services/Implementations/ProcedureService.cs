using AutoMapper;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Procedure;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

namespace HospitalManagement.Services.Implementations
{
    public class ProcedureService : IProcedureService
    {
        private readonly IProcedureRepository procedureRepository;
        private readonly IMapper mapper;
        private readonly ILogger<ProcedureService> logger;

        public ProcedureService(IProcedureRepository procedureRepository, IMapper mapper, ILogger<ProcedureService> logger)
        {
            this.procedureRepository = procedureRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Result<ProcedureCreateResponseDto>> CreateAsync(ProcedureCreateRequestDto request)
        {
            var procedureDomain = mapper.Map<Procedure>(request);

            procedureDomain = await procedureRepository.CreateAsync(procedureDomain);

            logger.LogInformation("Procedure created with id {id}", procedureDomain.Id);

            var result = mapper.Map<ProcedureCreateResponseDto>(procedureDomain);

            return Result<ProcedureCreateResponseDto>.Ok(result);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var procedureDomain = await procedureRepository.DeleteAsync(id);

            if(procedureDomain == null)
            {
                logger.LogWarning("Procedure with id {id} not found", id);
                return Result.Fail($"Procedure with id {id} not found", "INVALID_ID");
            }

            logger.LogInformation("Procedure with id {id} deleted", id);
            return Result.Ok("Procedure deleted");
        }

        public async Task<Result<List<ProcedureListDto>>> GetAllAsync()
        {
            var procedureListDomain = await procedureRepository.GetAllAsync();

            var result = mapper.Map<List<ProcedureListDto>>(procedureListDomain);

            return Result<List<ProcedureListDto>>.Ok(result);
        }

        public async Task<Result<ProcedureResponseDto>> GetByIdAsync(int id)
        {
            var procedureDomain = await procedureRepository.GetByIdAsync(id);

            if(procedureDomain == null)
            {
                logger.LogWarning("Procedure with id {id} not found", id);
                return Result<ProcedureResponseDto>.Fail($"Procedure with id {id} not found", "INVALID_ID");
            }

            var result = mapper.Map<ProcedureResponseDto>(procedureDomain);

            return Result<ProcedureResponseDto>.Ok(result);
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

            logger.LogInformation("Procedure with id {id} updated", procedureDomain.Id);

            var result = mapper.Map<ProcedureUpdateResponseDto>(procedureDomain);

            return Result<ProcedureUpdateResponseDto>.Ok(result);
        }
    }
}
