using AutoMapper;
using HospitalManagement.QueryService.Data;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.Shared.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Consumers
{
    public class DoctorConsumer :
        IConsumer<DoctorCreated>,
        IConsumer<DoctorUpdated>,
        IConsumer<DoctorDeleted>
    {
        private readonly QueryDbContext dbContext;
        private readonly ILogger<DoctorConsumer> logger;
        private readonly IMapper mapper;

        public DoctorConsumer(QueryDbContext dbContext, ILogger<DoctorConsumer> logger,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<DoctorCreated> context)
        {
            var message = context.Message;
            logger.LogInformation("DoctorCreated event received for doctor {Id}," +
                "CorrelationId: {CorrelationId}", message.Id, message.CorrelationId);

            var readModel = mapper.Map<DoctorReadModel>(message);

            await dbContext.Doctors.AddAsync(readModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Doctor {Id} added to read database", message.Id);
        }

        public async Task Consume(ConsumeContext<DoctorUpdated> context)
        {
            var message = context.Message;
            logger.LogInformation("DoctorUpdated event received for doctor {Id}," +
                "CorrelationId: {CorrelationId}", message.Id, message.CorrelationId);

            var readModel = await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == message.Id);

            if(readModel == null)
            {
                logger.LogWarning("Doctor {Id} not found in read database for update", message.Id);
                return;
            }

            readModel.FirstName = message.FirstName;
            readModel.LastName = message.LastName;
            readModel.Specialization = message.Specialization;
            readModel.Email = message.Email;
            readModel.Phone = message.Phone;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Doctor {Id} updated in read database", message.Id);
        }

        public async Task Consume(ConsumeContext<DoctorDeleted> context)
        {
            var message = context.Message;
            logger.LogInformation("DoctorDeleted event received for doctor {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == message.Id);

            if(readModel == null)
            {
                logger.LogWarning("Doctor {Id} not found in read database for deletion", message.Id);
                return;
            }

            dbContext.Doctors.Remove(readModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Doctor {Id} removed from read database", message.Id);
        }
    }
}
