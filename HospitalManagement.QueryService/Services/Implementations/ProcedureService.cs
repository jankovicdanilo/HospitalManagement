using AutoMapper;
using HospitalManagement.QueryService.Models.Procedure;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Implementations
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

        public async Task<Result<List<ProcedureListDto>>> GetAllAsync()
        {
            var procedures = await procedureRepository.GetAllAsync();
            var result = mapper.Map<List<ProcedureListDto>>(procedures);
            return Result<List<ProcedureListDto>>.Ok(result);
        }

        public async Task<Result<ProcedureResponseDto>> GetByIdAsync(int id)
        {
            var procedure = await procedureRepository.GetByIdAsync(id);
            if (procedure == null)
            {
                logger.LogWarning("Procedure with id {id} not found", id);
                return Result<ProcedureResponseDto>.Fail($"Procedure with id {id} not found", "INVALID_ID");
            }
            var result = mapper.Map<ProcedureResponseDto>(procedure);
            return Result<ProcedureResponseDto>.Ok(result);
        }
    }
}