using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.DoctorSchedule;
using HospitalManagement.Shared.Models.DTOs;

namespace HospitalManagement.Mappings
{
    public class DoctorScheduleProfile : Profile
    {
        public DoctorScheduleProfile()
        {
            CreateMap<DoctorSchedule, DoctorScheduleCreateRequestDto>().ReverseMap();
            CreateMap<DoctorSchedule, DoctorScheduleCreateResponseDto>().ReverseMap();
            CreateMap<DoctorSchedule, DoctorScheduleUpdateRequestDto>().ReverseMap();
            CreateMap<DoctorSchedule, DoctorScheduleUpdateResponseDto>().ReverseMap();
            CreateMap<DoctorSchedule, DoctorScheduleResponseDto>().ReverseMap();
        }
    }
}
