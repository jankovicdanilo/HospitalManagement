using AutoMapper;
using HospitalManagement.QueryService.Data;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.Shared.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Consumers
{
    public class ProcedureConsumer :
        IConsumer<ProcedureCreated>,
        IConsumer<ProcedureUpdated>,
        IConsumer<ProcedureDeleted>
    {
        private readonly QueryDbContext dbContext;
        private readonly ILogger<ProcedureConsumer> logger;
        private readonly IMapper mapper;

        public ProcedureConsumer(QueryDbContext dbContext, ILogger<ProcedureConsumer> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<ProcedureCreated> context)
        {
            var message = context.Message;
            logger.LogInformation("ProcedureCreated event received for procedure {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = mapper.Map<ProcedureReadModel>(message);

            await dbContext.Procedures.AddAsync(readModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Procedure {Id} added to read database", message.Id);
        }
    
        public async Task Consume(ConsumeContext<ProcedureUpdated> context)
        {
            var message = context.Message;
            logger.LogInformation("ProcedureUpdated event received for procedure {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = await dbContext.Procedures.FirstOrDefaultAsync(x => x.Id == message.Id);
            if (readModel == null)
            {
                logger.LogWarning("Procedure {Id} not found in read database for update", message.Id);
                return;
            }

            readModel.Name = message.Name;
            readModel.Price = message.Price;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Procedure {Id} updated in read database", message.Id);
        }

        public async Task Consume(ConsumeContext<ProcedureDeleted> context)
        {
            var message = context.Message;
            logger.LogInformation("ProcedureDeleted event received for procedure {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = await dbContext.Procedures.FirstOrDefaultAsync(x => x.Id == message.Id);
            if (readModel == null)
            {
                logger.LogWarning("Procedure {Id} not found in read database for deletion", message.Id);
                return;
            }

            dbContext.Procedures.Remove(readModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Procedure {Id} removed from read database", message.Id);
        }
    }
}
