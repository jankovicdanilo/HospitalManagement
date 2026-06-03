namespace HospitalManagement.Models.DTOs.Procedure
{
    public class ProcedureResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
