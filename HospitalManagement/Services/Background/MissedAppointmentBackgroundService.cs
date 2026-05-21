using HospitalManagement.Models.Enums;
using HospitalManagement.Repositories.Interfaces;

namespace HospitalManagement.Services.Background
{
    public class MissedAppointmentBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public MissedAppointmentBackgroundService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using(var scope = scopeFactory.CreateScope())
                {
                    var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();

                    var missedAppointments = await appointmentRepository.GetPendingPastAppointmentsAsync();

                    foreach(var appointment in missedAppointments)
                    {
                        appointment.Status = AppointmentStatus.Missed;
                        await appointmentRepository.UpdateAsync(appointment);
                    }
                }

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }
    }
}
