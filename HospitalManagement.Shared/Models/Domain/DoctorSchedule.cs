namespace HospitalManagement.Shared.Models.Domain
{
    public class DoctorSchedule
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int  StartHour { get; set; }
        public int EndHour { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
    }
}
