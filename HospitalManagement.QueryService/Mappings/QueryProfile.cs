using AutoMapper;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.QueryService.Mappings
{
    public class QueryProfile : Profile
    {
        public QueryProfile()
        {
            // Doctor
            CreateMap<Doctor, DoctorResponseDto>();

            // Patient
            CreateMap<Patient, PatientListDto>();
            CreateMap<Patient, PatientGetByIdDto>();
            CreateMap<Patient, PatientMedicalHistoryDto>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Name + " " + src.LastName));

            // Procedure
            CreateMap<Procedure, ProcedureListDto>();
            CreateMap<Procedure, ProcedureResponseDto>();

            // DoctorSchedule
            CreateMap<DoctorSchedule, DoctorScheduleResponseDto>();
        }
    }
}