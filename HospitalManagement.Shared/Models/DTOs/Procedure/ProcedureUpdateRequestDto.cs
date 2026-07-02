namespace HospitalManagement.Shared.Models.DTOs.Procedure
{
    public class ProcedureUpdateRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
