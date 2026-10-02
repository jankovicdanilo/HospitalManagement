namespace HospitalManagement.Shared.Models.DTOs.Statistics
{
    public class AppointmentStatsProcedureDto
    {
        public int ProcedureId { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public decimal ProcedurePrice { get; set; }
    }
}
