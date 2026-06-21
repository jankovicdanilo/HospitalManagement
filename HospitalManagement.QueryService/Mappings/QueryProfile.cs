using AutoMapper;
using HospitalManagement.QueryService.Models.Doctor;
using HospitalManagement.QueryService.Models.DTOs.DoctorSchedule;
using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.QueryService.Models.Procedure;
using HospitalManagement.QueryService.Models.ReadModels;

namespace HospitalManagement.QueryService.Mappings
{
    public class QueryProfile : Profile
    {
        public QueryProfile()
        {
            // Doctor
            CreateMap<DoctorReadModel, DoctorResponseDto>();

            // Patient
            CreateMap<PatientReadModel, PatientListDto>();
            CreateMap<PatientReadModel, PatientGetByIdDto>();
            CreateMap<PatientReadModel, PatientMedicalHistoryDto>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Name + " " + src.LastName));

            // Procedure
            CreateMap<ProcedureReadModel, ProcedureListDto>();
            CreateMap<ProcedureReadModel, ProcedureResponseDto>();

            // DoctorSchedule
            CreateMap<DoctorScheduleReadModel, DoctorScheduleResponseDto>();
        }
    }
}