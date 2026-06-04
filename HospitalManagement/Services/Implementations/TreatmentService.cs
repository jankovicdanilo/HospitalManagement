using AutoMapper;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Treatment;
using HospitalManagement.Models.Enums;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Services.Validations;

namespace HospitalManagement.Services.Implementations
{
    public class TreatmentService : ITreatmentService
    {
        private readonly ITreatmentRepository treatmentRepository;
        private readonly ITreatmentValidation treatmentValidation;
        private readonly IMapper mapper;
        private readonly ILogger<TreatmentService> logger;

        public TreatmentService(ITreatmentRepository treatmentRepository, IMapper mapper,
            IAppointmentRepository appointmentRepository, ITreatmentValidation treatmentValidation, 
            ILogger<TreatmentService> logger)
        {
            this.treatmentRepository = treatmentRepository;
            this.mapper = mapper;
            this.treatmentValidation = treatmentValidation;
            this.logger = logger;
        }

        public async Task<Result<TreatmentCreateResponseDto>> CreateAsync(TreatmentCreateRequestDto request)
        {
            var validate = await treatmentValidation.ValidateAll(request);

            if (!validate.Success)
            {
                logger.LogWarning("Treatment creation failed {Message}", validate.Message);
                return Result<TreatmentCreateResponseDto>.Fail(validate.Message, validate.ErrorCode);
            }

            var treatmentDomain = mapper.Map<Treatment>(request); 

            treatmentDomain = await treatmentRepository.CreateAsync(treatmentDomain);

            logger.LogInformation("Treatment created with id {id}", treatmentDomain.Id);

            var result = mapper.Map<TreatmentCreateResponseDto>(treatmentDomain);

            return Result<TreatmentCreateResponseDto>.Ok(result);
        }
    }
}
