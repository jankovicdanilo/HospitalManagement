namespace HospitalManagement.Appointments.Models.Domain
{
    public class AppointmentProcedure
    {
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        public int ProcedureId { get; set; }

        // Snapshot fields — captured at creation time from the main API's
        // Procedure catalog, since Procedure lives in a separate database now.
        public string ProcedureName { get; set; } = null!;

        public decimal ProcedurePrice { get; set; }
    }
}