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
        private readonly TreatmentValidation treatmentValidation;
        private readonly IMapper mapper;

        public TreatmentService(ITreatmentRepository treatmentRepository, IMapper mapper,
            IAppointmentRepository appointmentRepository, TreatmentValidation treatmentValidation)
        {
            this.treatmentRepository = treatmentRepository;
            this.mapper = mapper;
            this.treatmentValidation = treatmentValidation;
        }

        public async Task<Result<TreatmentCreateResponseDto>> CreateAsync(TreatmentCreateRequestDto request)
        {
            var validate = await treatmentValidation.ValidateAll(request);

            if (!validate.Success)
            {
                return Result<TreatmentCreateResponseDto>.Fail(validate.Message, validate.ErrorCode);
            }

            var treatmentDomain = mapper.Map<Treatment>(request); 

            treatmentDomain = await treatmentRepository.CreateAsync(treatmentDomain);

            var result = mapper.Map<TreatmentCreateResponseDto>(treatmentDomain);

            return Result<TreatmentCreateResponseDto>.Ok(result);
        }
    }
}
