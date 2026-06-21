namespace HospitalManagement.CommandService.Models.DTOs.DoctorSchedule
{
    public class DoctorScheduleUpdateRequestDto
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
    }
}
