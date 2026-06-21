using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Events
{
    public record DoctorScheduleCreated
    (
        Guid CorrelationId,
        int Id,
        int DoctorId,
        DayOfWeek DayOfWeek,
        int StartHour,
        int EndHour
    );

    public record DoctorScheduleUpdated
    (
        Guid CorrelationId,
        int Id,
        int DoctorId,
        DayOfWeek DayOfWeek,
        int StartHour,
        int EndHour
    );

    public record DoctorScheduleDeleted
    (
        Guid CorrelationId,
        int Id
    );
}
