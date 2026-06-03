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
                    opt => opt.MapFrom(src => src.Patient.Name + " " + src.Patient.LastName))
                .ForMember(dest => dest.Procedures, 
                    opt => opt.MapFrom(src => src.AppointmentProcedures.Select(ap => ap.Procedure)))
                .ForMember(dest => dest.TotalCost, 
                    opt => opt.MapFrom(src => src.AppointmentProcedures.Sum(ap => ap.Procedure.Price)));

            CreateMap<Appointment, AppointmentHistoryDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName));

            CreateMap<Appointment, AppointmentRequestDto>().ReverseMap();

            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.Procedures,
                    opt => opt.MapFrom(src => src.AppointmentProcedures.Select(ap => ap.Procedure)))
                .ForMember(dest => dest.TotalCost,
                    opt => opt.MapFrom(src => src.AppointmentProcedures.Sum(ap => ap.Procedure.Price)));
            CreateMap<Appointment, AppointmentUpdateRequestDto>().ReverseMap();

            CreateMap<Appointment, AppointmentUpdateResponseDto>()
                .ForMember(dest => dest.Procedures,
                    opt => opt.MapFrom(src => src.AppointmentProcedures.Select(ap => ap.Procedure)))
                .ForMember(dest => dest.TotalCost,
                    opt => opt.MapFrom(src => src.AppointmentProcedures.Sum(ap => ap.Procedure.Price)));
            CreateMap<Appointment, AppointmentCreateRequestDto>().ReverseMap();

            CreateMap<Appointment, AppointmentCreateResponseDto>()
                .ForMember(dest => dest.Procedures,
                    opt => opt.MapFrom(src => src.AppointmentProcedures.Select(ap => ap.Procedure)))
                .ForMember(dest => dest.TotalCost,
                    opt => opt.MapFrom(src => src.AppointmentProcedures.Sum(ap => ap.Procedure.Price)));
        }
    }
}
