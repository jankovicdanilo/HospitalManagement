namespace HospitalManagement.Appointments.Models.DTOs.External
{
    public class ExternalDoctorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class ExternalPatientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ExternalProcedureDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class ExternalDoctorScheduleDto
    {
        public int StartHour { get; set; }
        public int EndHour { get; set; }
    }
}