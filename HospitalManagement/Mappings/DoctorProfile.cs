using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Doctor;

namespace HospitalManagement.Mappings
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, DoctorCreateRequestDto>().ReverseMap();
            CreateMap<Doctor, DoctorResponseDto>().ReverseMap();
            CreateMap<Doctor, DoctorUpdateRequestDto>().ReverseMap();
        }
    }
}
