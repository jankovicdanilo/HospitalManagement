using AutoMapper;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Procedure;

namespace HospitalManagement.CommandService.Mappings
{
    public class CommandProfile : Profile
    {
        public CommandProfile()
        {
            // Doctor
            CreateMap<Doctor, DoctorCreateRequestDto>().ReverseMap();
            CreateMap<Doctor, DoctorResponseDto>().ReverseMap();
            CreateMap<Doctor, DoctorUpdateRequestDto>().ReverseMap();

            // Patient
            CreateMap<Patient, PatientCreateRequestDto>().ReverseMap();
            CreateMap<Patient, PatientCreateResponseDto>().ReverseMap();
            CreateMap<Patient, PatientUpdateRequestDto>().ReverseMap();
            CreateMap<Patient, PatientUpdateResponseDto>().ReverseMap();

            // Procedure
            CreateMap<Procedure, ProcedureCreateRequestDto>().ReverseMap();
            CreateMap<Procedure, ProcedureCreateResponseDto>().ReverseMap();
            CreateMap<Procedure, ProcedureUpdateRequestDto>().ReverseMap();
            CreateMap<Procedure, ProcedureUpdateResponseDto>().ReverseMap();

            // DoctorSchedule
            CreateMap<DoctorSchedule, DoctorScheduleCreateRequestDto>().ReverseMap();
            CreateMap<DoctorSchedule, DoctorScheduleCreateResponseDto>().ReverseMap();
            CreateMap<DoctorSchedule, DoctorScheduleUpdateRequestDto>().ReverseMap();
            CreateMap<DoctorSchedule, DoctorScheduleUpdateResponseDto>().ReverseMap();
        }
    }
}