using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Events
{
    public record ProcedureCreated
    (
        Guid CorrelationId,
        int Id,
        string Name,
        decimal Price
    );

    public record ProcedureUpdated
    (
        Guid CorrelationId,
        int Id,
        string Name,
        decimal Price
    );

    public record ProcedureDeleted
    (
        Guid CorrelationId,
        int Id
    );
}
