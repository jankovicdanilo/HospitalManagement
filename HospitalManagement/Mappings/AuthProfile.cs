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
            CreateMap<User, AuthResponseDto>();
            CreateMap<User, AuthResponseListDto>();
            CreateMap<User, AuthResponseUpdateDto>();
            CreateMap<User, CurrentUserDto>();
            CreateMap<RegisterRequestDto, User>();
            CreateMap<UpdateUserRequestDto, User>();
        }
    }
}
