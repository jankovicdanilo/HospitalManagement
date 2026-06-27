using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs;

namespace HospitalManagement.Mappings
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientListDto>().ReverseMap();
            CreateMap<Patient, PatientResponseDto>().ReverseMap();
            CreateMap<Patient, PatientCreateRequestDto>().ReverseMap();
            CreateMap<Patient, PatientCreateResponseDto>().ReverseMap();
            CreateMap<Patient, PatientUpdateRequestDto>().ReverseMap();
            CreateMap<Patient, PatientUpdateResponseDto>().ReverseMap();
            CreateMap<Patient, PatientMedicalHistoryDto>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Name + " " + src.LastName));
        }
    }
}
