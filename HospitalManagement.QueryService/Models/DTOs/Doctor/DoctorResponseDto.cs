namespace HospitalManagement.QueryService.Models.Doctor
{
    public class DoctorResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }
}
