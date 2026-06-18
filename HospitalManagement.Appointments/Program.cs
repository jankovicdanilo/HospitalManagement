using HospitalManagement.Appointments.Data;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<AppointmentDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentDb")));

    builder.Services.AddAutoMapper(typeof(Program));

    // TODO: register IAppointmentRepository/AppointmentRepository,
    // IAppointmentProcedureRepository/AppointmentProcedureRepository,
    // IAppointmentService/AppointmentService,
    // IAppointmentProcedureService/AppointmentProcedureService,
    // IAppointmentValidation/AppointmentValidation,
    // IAppointmentDiscountCalculator/AppointmentDiscountCalculator
    // once those files are moved over

    // TODO: JWT auth setup (copy from main API's Program.cs, since appointment
    // endpoints will presumably still require authentication)

    builder.Services.Configure<DiscountSettings>(builder.Configuration.GetSection("DiscountSettings"));
    builder.Services.Configure<AppointmentSettings>(builder.Configuration.GetSection("AppointmentSettings"));

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
        db.Database.Migrate();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}