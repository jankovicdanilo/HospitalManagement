namespace HospitalManagement.Appointments.Models.DTOs.Appointment
{
    public class AppointmentSettings
    {
        public int WorkStartHour { get; set; }
        public int WorkEndHour { get; set; }
        public int SlotSizeMinutes { get; set; }
    }
}