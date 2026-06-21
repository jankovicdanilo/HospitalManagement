namespace HospitalManagement.CommandService.Models.DTOs.DoctorSchedule
{
    public class DoctorScheduleUpdateResponseDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
    }
}
