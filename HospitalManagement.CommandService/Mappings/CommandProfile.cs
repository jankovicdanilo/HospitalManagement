using AutoMapper;
using HospitalManagement.CommandService.Models.Doctor;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Models.DTOs.Doctor;
using HospitalManagement.CommandService.Models.DTOs.DoctorSchedule;
using HospitalManagement.CommandService.Models.Patient;
using HospitalManagement.CommandService.Models.Procedure;

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