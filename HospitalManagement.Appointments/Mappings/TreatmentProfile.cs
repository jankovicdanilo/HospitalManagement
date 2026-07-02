using AutoMapper;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Treatment;

namespace HospitalManagement.Appointments.Mappings
{
    public class TreatmentProfile : Profile
    {
        public TreatmentProfile()
        {
            CreateMap<Treatment, TreatmentCreateRequestDto>().ReverseMap();
            CreateMap<Treatment, TreatmentCreateResponseDto>().ReverseMap();
            CreateMap<Treatment, TreatmentResponseDto>().ReverseMap();
        }
    }
}