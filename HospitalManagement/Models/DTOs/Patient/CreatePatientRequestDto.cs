namespace HospitalManagement.Models.DTOs.Patient
{
    public class CreatePatientRequestDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
