namespace HospitalManagement.Models.Domain
{
    public class AppointmentProcedure
    {
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        public int ProcedureId { get; set; }
        public Procedure Procedure { get; set; } = null!;
    }
}
