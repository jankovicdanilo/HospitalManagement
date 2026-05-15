using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;

namespace HospitalManagement.Mappings
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentListResponseDto>();
            CreateMap<Appointment, AppointmentResponseDto>();
            CreateMap<Appointment, AppointmentUpdateResponseDto>();
            CreateMap<AppointmentUpdateRequestDto, Appointment>();
        }
    }
}
