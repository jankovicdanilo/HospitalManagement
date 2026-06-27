using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Appointments.Repositories.Interfaces;

namespace HospitalManagement.Appointments.Services.Background
{
    public class MissedAppointmentBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<MissedAppointmentBackgroundService> logger;

        public MissedAppointmentBackgroundService(IServiceScopeFactory scopeFactory,
            ILogger<MissedAppointmentBackgroundService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Missed appointment check started");

                using (var scope = scopeFactory.CreateScope())
                {
                    var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();

                    var missedAppointments = await appointmentRepository.GetPendingPastAppointmentsAsync();

                    foreach (var appointment in missedAppointments)
                    {
                        appointment.Status = AppointmentStatus.Missed;
                        await appointmentRepository.UpdateAsync(appointment);
                        logger.LogInformation("Appointment with id {Id} marked as Missed", appointment.Id);
                    }

                    logger.LogInformation("Missed appointments check completed. {Count} " +
                        "appointments marked as Missed", missedAppointments.Count());
                }

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }
    }
}