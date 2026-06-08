using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.AppointmentProcedure;

namespace HospitalManagement.Mappings
{
    public class AppointmentProcedureProfile : Profile
    {
        public AppointmentProcedureProfile()
        {
            CreateMap<AppointmentProcedure, AppointmentProcedureResponseDto>().ReverseMap();
            CreateMap<AppointmentProcedure, AppointmentProcedureCreateResponseDto>().ReverseMap();
            CreateMap<AppointmentProcedure, AppointmentProcedureCreateRequestDto>().ReverseMap();
        }
    }
}
