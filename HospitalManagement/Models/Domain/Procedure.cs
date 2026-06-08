namespace HospitalManagement.Models.Domain
{
    public class Procedure
    {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
        public ICollection<AppointmentProcedure> AppointmentProcedures { get; set; } = [];
    }
}
