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
            CreateMap<Patient, CreatePatientRequestDto>().ReverseMap();
            CreateMap<Patient, CreatePatientResponseDto>().ReverseMap();
        }
    }
}
