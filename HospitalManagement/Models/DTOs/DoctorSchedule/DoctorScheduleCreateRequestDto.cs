namespace HospitalManagement.Models.DTOs.DoctorSchedule
{
    public class DoctorScheduleCreateRequestDto
    {
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
    }
}
