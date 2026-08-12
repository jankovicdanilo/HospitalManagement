namespace HospitalManagement.Appointments.Services.Interfaces
{
    public interface IClinicTimeZoneProvider
    {
        DateTime ToLocal(DateTime utc);
        DateTime ToUtc(DateTime local);
    }
}
