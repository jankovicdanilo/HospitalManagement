namespace HospitalManagement.QueryService.Models.ReadModels
{
    public class ProcedureReadModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
    }
}