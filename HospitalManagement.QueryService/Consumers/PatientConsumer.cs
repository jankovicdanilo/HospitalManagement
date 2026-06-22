using AutoMapper;
using HospitalManagement.QueryService.Data;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.Shared.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Consumers
{
    public class PatientConsumer :
        IConsumer<PatientCreated>,
        IConsumer<PatientUpdated>,
        IConsumer<PatientDeleted>
    {
        private readonly QueryDbContext dbContext;
        private readonly ILogger<PatientConsumer> logger;
        private readonly IMapper mapper;

        public PatientConsumer(QueryDbContext dbContext, ILogger<PatientConsumer> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<PatientCreated> context)
        {
            var message = context.Message;
            logger.LogInformation("PatientCreated event received for patient {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = mapper.Map<PatientReadModel>(message);

            await dbContext.Patients.AddAsync(readModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Patient {Id} added to read database", message.Id);
        }

        public async Task Consume(ConsumeContext<PatientUpdated> context)
        {
            var message = context.Message;
            logger.LogInformation("PatientUpdated event received for patient {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == message.Id);

            if(readModel == null)
            {
                logger.LogWarning("Patient {Id} not found in read database for update", message.Id);
                return;
            }

            readModel.Name = message.Name;
            readModel.LastName = message.LastName;
            readModel.Email = message.Email;
            readModel.Phone = message.Phone;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Patient {Id} updated in read database", message.Id);
        }

        public async Task Consume(ConsumeContext<PatientDeleted> context)
        {
            var message = context.Message;
            logger.LogInformation("PatientDeleted event received for patient {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == message.Id);

            if(readModel == null)
            {
                logger.LogWarning("Patient {Id} not found in read database for deletion", message.Id);
                return;
            }

            dbContext.Patients.Remove(readModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Patient {Id} removed from read database", message.Id);
        }
    
    }
}
