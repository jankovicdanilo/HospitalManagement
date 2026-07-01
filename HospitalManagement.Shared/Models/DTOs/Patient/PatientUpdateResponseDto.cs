namespace HospitalManagement.Shared.Models.DTOs.Patient
{
    public class PatientUpdateResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
