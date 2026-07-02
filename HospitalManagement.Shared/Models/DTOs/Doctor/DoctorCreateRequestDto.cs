namespace HospitalManagement.Shared.Models.DTOs.Doctor
{
    public class DoctorCreateRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }
}
