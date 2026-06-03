namespace HospitalManagement.Models.DTOs.Procedure
{
    public class ProcedureUpdateResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
