namespace HospitalManagement.QueryService.Models.DTOs.Patient
{
    public class PatientListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}