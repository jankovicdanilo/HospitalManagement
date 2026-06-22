using AutoMapper;
using HospitalManagement.QueryService.Data;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.Shared.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Consumers
{
    public class DoctorScheduleConsumer :
        IConsumer<DoctorScheduleCreated>,
        IConsumer<DoctorScheduleUpdated>,
        IConsumer<DoctorScheduleDeleted>
    {
        private readonly QueryDbContext dbContext;
        private readonly ILogger<DoctorScheduleConsumer> logger;
        private readonly IMapper mapper;

        public DoctorScheduleConsumer(QueryDbContext dbContext, ILogger<DoctorScheduleConsumer> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<DoctorScheduleCreated> context)
        {
            var message = context.Message;
            logger.LogInformation("DoctorScheduleCreated event received for schedule {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = mapper.Map<DoctorScheduleReadModel>(message);
            await dbContext.DoctorSchedules.AddAsync(readModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("DoctorSchedule {Id} added to read database", message.Id);
        }
    
        public async Task Consume(ConsumeContext<DoctorScheduleUpdated> context)
        {
            var message = context.Message;
            logger.LogInformation("DoctorScheduleUpdated event received for schedule {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = await dbContext.DoctorSchedules.FirstOrDefaultAsync(x => x.Id == message.Id);

            if(readModel == null)
            {
                logger.LogWarning("DoctorSchedule {Id} not found in read database for update", message.Id);
                return;
            }

            readModel.DayOfWeek = message.DayOfWeek;
            readModel.StartHour = message.StartHour;
            readModel.EndHour = message.EndHour;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("DoctorSchedule {Id} updated in read database", message.Id);
        }

        public async Task Consume(ConsumeContext<DoctorScheduleDeleted> context)
        {
            var message = context.Message;
            logger.LogInformation("DoctorScheduleDeleted event received for schedule {Id}, CorrelationId: {CorrelationId}",
                message.Id, message.CorrelationId);

            var readModel = await dbContext.DoctorSchedules.FirstOrDefaultAsync(x => x.Id ==message.Id);
            if(readModel == null)
            {
                logger.LogWarning("DoctorSchedule {Id} not found in read database for deletion", message.Id);
                return;
            }

            dbContext.DoctorSchedules.Remove(readModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("DoctorSchedule {Id} removed from read database", message.Id);
        }
    }
}
