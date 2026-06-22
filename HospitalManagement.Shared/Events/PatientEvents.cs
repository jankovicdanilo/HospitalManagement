using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Events
{
    public record PatientCreated
    (
        Guid CorrelationId,
        int Id,
        string Name,
        string LastName,
        string Email,
        string? Phone,
        DateOnly DateOfBirth
    );

    public record PatientUpdated
    (
        Guid CorrelationId,
        int Id,
        string Name,
        string LastName,
        string Email,
        string? Phone
    );

    public record PatientDeleted
    (
        Guid CorrelationId,
        int Id
    );
}
