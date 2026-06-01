using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Models.DTOs.Patient;

namespace HospitalManagement.Mappings
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentListResponseDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient.Name + " " + src.Patient.LastName));
            CreateMap<Appointment, AppointmentHistoryDto>()
                .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName));
            CreateMap<Appointment, AppointmentRequestDto>().ReverseMap();
            CreateMap<Appointment, AppointmentResponseDto>().ReverseMap();
            CreateMap<Appointment, AppointmentUpdateRequestDto>().ReverseMap();
            CreateMap<Appointment, AppointmentUpdateResponseDto>().ReverseMap();
            CreateMap<Appointment, AppointmentCreateRequestDto>().ReverseMap();
            CreateMap<Appointment, AppointmentCreateResponseDto>().ReverseMap();
        }
    }
}
