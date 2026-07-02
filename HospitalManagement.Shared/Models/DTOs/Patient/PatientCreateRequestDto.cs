namespace HospitalManagement.Shared.Models.DTOs.Patient
{
    public class PatientCreateRequestDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
