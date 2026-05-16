using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Models.DTOs.Auth;

namespace HospitalManagement.Mappings
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<User, AuthResponseDto>().ReverseMap();
            CreateMap<User, AuthResponseListDto>().ReverseMap();
            CreateMap<User, AuthResponseUpdateDto>().ReverseMap();
            CreateMap<User, CurrentUserDto>().ReverseMap();
            CreateMap<User, RegisterRequestDto>().ReverseMap();
            CreateMap<User, UpdateUserRequestDto>().ReverseMap();
        }
    }
}
