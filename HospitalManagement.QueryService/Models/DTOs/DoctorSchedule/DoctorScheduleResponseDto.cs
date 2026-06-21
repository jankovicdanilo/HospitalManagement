namespace HospitalManagement.QueryService.Models.DTOs.DoctorSchedule
{
    public class DoctorScheduleResponseDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
    }
}