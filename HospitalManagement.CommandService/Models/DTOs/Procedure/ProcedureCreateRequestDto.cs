namespace HospitalManagement.CommandService.Models.Procedure
{
    public class ProcedureCreateRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
