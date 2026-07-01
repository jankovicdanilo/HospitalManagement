namespace HospitalManagement.QueryService.Models.DTOs.Patient
{
    public class AppointmentProcedureDto
    {
        public int AppointmentId { get; set; }
        public int ProcedureId { get; set; }
        public string ProcedureName { get; set; } = null!;
        public decimal ProcedurePrice { get; set; }
    }
}