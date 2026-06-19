using AutoMapper;
using HospitalManagement.Auth.Models.Domain;
using HospitalManagement.Auth.Models.DTOs;

namespace HospitalManagement.Auth.Mappings
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
