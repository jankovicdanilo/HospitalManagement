using AutoMapper;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;

namespace HospitalManagement.Appointments.Mappings
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