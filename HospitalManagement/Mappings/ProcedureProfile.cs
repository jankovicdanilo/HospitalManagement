using AutoMapper;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.Shared.Models.DTOs;

namespace HospitalManagement.Mappings
{
    public class ProcedureProfile : Profile
    {
        public ProcedureProfile()
        {
            CreateMap<Procedure, ProcedureListDto>().ReverseMap();
            CreateMap<Procedure, ProcedureResponseDto>().ReverseMap();
            CreateMap<Procedure, ProcedureCreateRequestDto>().ReverseMap();
            CreateMap<Procedure, ProcedureCreateResponseDto>().ReverseMap();
            CreateMap<Procedure, ProcedureUpdateRequestDto>().ReverseMap();
            CreateMap<Procedure, ProcedureUpdateResponseDto>().ReverseMap();
        }
    }
}
