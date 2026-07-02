namespace HospitalManagement.QueryService.Models.DTOs.Patient
{
    public class PatientGetByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
    }
}