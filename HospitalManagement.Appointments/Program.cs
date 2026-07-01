using HospitalManagement.Appointments.Data;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Repositories.Implementations;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Background;
using HospitalManagement.Appointments.Services.Calculators.Implementations;
using HospitalManagement.Appointments.Services.Calculators.Interfaces;
using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Appointments.Services.Validations;
using HospitalManagement.Appointments.Settings;
using HospitalManagement.Shared.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

// Repositories
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentProcedureRepository, AppointmentProcedureRepository>();
builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();

// Services
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAppointmentProcedureService, AppointmentProcedureService>();
builder.Services.AddScoped<ITreatmentService, TreatmentService>();

// Validations
builder.Services.AddScoped<IAppointmentValidation, AppointmentValidation>();
builder.Services.AddScoped<IAppointmentProcedureValidation, AppointmentProcedureValidation>();
builder.Services.AddScoped<ITreatmentValidation, TreatmentValidation>();

// Calculators
builder.Services.AddScoped<IAppointmentDiscountCalculator, AppointmentDiscountCalculator>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthTokenHandler>();

//HTTP client for cross-service calls to main API
builder.Services.AddHttpClient<IHospitalManagementClient, HospitalManagementClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["HospitalManagement_BaseUrl"]!);
})
    .AddHttpMessageHandler<HospitalManagement.Shared.Http.AuthTokenHandler>();

// Background services
builder.Services.AddHostedService<MissedAppointmentBackgroundService>();

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