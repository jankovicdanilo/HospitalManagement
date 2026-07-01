using AutoMapper;
using HospitalManagement.QueryService.Models.Doctor;
using HospitalManagement.QueryService.Models.DTOs.DoctorSchedule;
using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.QueryService.Models.Procedure;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.Shared.Events;

namespace HospitalManagement.QueryService.Mappings
{
    public class QueryProfile : Profile
    {
        public QueryProfile()
        {
            // Doctor
            CreateMap<DoctorReadModel, DoctorResponseDto>();
            CreateMap<DoctorCreated, DoctorReadModel>();
            CreateMap<DoctorUpdated, DoctorReadModel>();

            // Patient
            CreateMap<PatientReadModel, PatientListDto>();
            CreateMap<PatientReadModel, PatientGetByIdDto>();
            CreateMap<PatientReadModel, PatientMedicalHistoryDto>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Name + " " + src.LastName));
            CreateMap<PatientCreated, PatientReadModel>();
            CreateMap<PatientUpdated, PatientReadModel>();

            // Procedure
            CreateMap<ProcedureReadModel, ProcedureListDto>();
            CreateMap<ProcedureReadModel, ProcedureResponseDto>();
            CreateMap<ProcedureCreated, ProcedureReadModel>();
            CreateMap<ProcedureUpdated, ProcedureReadModel>();

            // DoctorSchedule
            CreateMap<DoctorScheduleReadModel, DoctorScheduleResponseDto>();
            CreateMap<DoctorScheduleCreated, DoctorScheduleReadModel>();
            CreateMap<DoctorScheduleUpdated, DoctorScheduleReadModel>();
        }
    }
}