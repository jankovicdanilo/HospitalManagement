namespace HospitalManagement.Models.DTOs.AppointmentProcedure
{
    public class AppointmentProcedureCreateResponseDto
    {
        public int AppointmentId { get; set; }
        public int ProcedureId { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public decimal ProcedurePrice { get; set; }
    }
}
