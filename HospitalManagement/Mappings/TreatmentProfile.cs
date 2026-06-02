using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Models.DTOs.Treatment;

namespace HospitalManagement.Mappings
{
    public class TreatmentProfile : Profile
    {
        public TreatmentProfile()
        {
            CreateMap<Treatment, TreatmentCreateRequestDto>().ReverseMap();
            CreateMap<Treatment, TreatmentCreateResponseDto>().ReverseMap();
            CreateMap<Treatment, TreatmentHistoryDto>().ReverseMap();
        }
    }
}
