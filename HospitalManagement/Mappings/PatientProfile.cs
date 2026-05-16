using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;

namespace HospitalManagement.Mappings
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientListDto>().ReverseMap();
            CreateMap<Patient, PatientGetByIdDto>().ReverseMap();
            CreateMap<Patient, PatientCreateRequestDto>().ReverseMap();
            CreateMap<Patient, PatientCreateResponseDto>().ReverseMap();
            CreateMap<Patient, PatientUpdateRequestDto>().ReverseMap();
            CreateMap<Patient, PatientUpdateResponseDto>().ReverseMap();
        }
    }
}
