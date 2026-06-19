using AutoMapper;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;

namespace HospitalManagement.Appointments.Mappings
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentListResponseDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.DoctorName))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.PatientName))
                .ForMember(dest => dest.Procedures,
                    opt => opt.MapFrom(src => src.AppointmentProcedures));

            CreateMap<Appointment, AppointmentRequestDto>().ReverseMap();

            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.DoctorName))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.PatientName))
                .ForMember(dest => dest.Procedures,
                    opt => opt.MapFrom(src => src.AppointmentProcedures));

            CreateMap<Appointment, AppointmentUpdateRequestDto>().ReverseMap();

            CreateMap<Appointment, AppointmentUpdateResponseDto>()
                .ForMember(dest => dest.Procedures,
                    opt => opt.MapFrom(src => src.AppointmentProcedures));

            CreateMap<Appointment, AppointmentCreateRequestDto>().ReverseMap();

            CreateMap<Appointment, AppointmentCreateResponseDto>()
                .ForMember(dest => dest.Procedures,
                    opt => opt.MapFrom(src => src.AppointmentProcedures));
        }
    }
}