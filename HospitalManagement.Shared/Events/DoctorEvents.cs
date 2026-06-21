using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Events
{
    public record DoctorCreated
    (
        Guid CorrelationId,
        int Id,
        string FirstName,
        string LastName,
        string Specialization,
        string Email,
        string? Phone
    );

    public record DoctorUpdated
    (
        Guid CorrelationId,
        int Id,
        string FirstName,
        string LastName,
        string Specialization,
        string Email,
        string? Phone
    );

    public record DoctorDeleted
    (
        Guid CorrelationId,
        int Id
    );
}
