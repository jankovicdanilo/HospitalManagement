namespace HospitalManagement.Shared.Models.Domain
{
    public class Procedure
    {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
    }
}
