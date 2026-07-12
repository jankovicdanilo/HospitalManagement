using AutoMapper;
using HospitalManagement.InvoiceService.Models.DTOs;
using HospitalManagement.InvoiceService.Models.DTOs.Invoice;

namespace HospitalManagement.InvoiceService.Mappings
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<AppointmentInvoiceDto, InvoiceData>()
                .ForMember(dest => dest.InvoiceNumber,
                    opt => opt.MapFrom(src => $"INV-{DateTime.UtcNow:yyyy}-{src.Id:D5}"))
                .ForMember(dest => dest.IssuedDate,
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => $"{src.Patient!.Name} {src.Patient.LastName}"))
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => $"{src.Doctor!.FirstName} {src.Doctor.LastName}"))
                .ForMember(dest => dest.AppointmentDate,
                    opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.Procedures,
                    opt => opt.MapFrom(src => src.Procedures.Select(p => new InvoiceLineItem
                    {
                        Name = p.ProcedureName,
                        Price = p.ProcedurePrice
                    }).ToList()))
                .ForMember(dest => dest.Subtotal,
                    opt => opt.MapFrom(src => src.Procedures.Sum(p => p.ProcedurePrice)))
                .ForMember(dest => dest.TotalAmount,
                    opt => opt.MapFrom(src => src.TotalCost));
        }
    }
}
