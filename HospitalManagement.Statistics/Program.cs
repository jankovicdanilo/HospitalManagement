using HospitalManagement.Shared.Extensions;
using HospitalManagement.Shared.Http;
using HospitalManagement.Shared.Settings;
using HospitalManagement.Statistics.Clients.Implementations;
using HospitalManagement.Statistics.Clients.Interfaces;
using HospitalManagement.Statistics.Services.Implementations;
using HospitalManagement.Statistics.Services.Interfaces;
using HospitalManagement.Statistics.Services.Utility;
using HospitalManagement.Statistics.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using QuestPDF.Infrastructure;
using System.Text;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<AuthTokenHandler>();

    builder.Services.AddScoped<StatisticsBuilder>();
    builder.Services.AddScoped<StatisticsCache>();
    builder.Services.AddScoped<IStatisticsService, StatisticsService>();

    // HTTP client for cross-service calls to Appointments
    builder.Services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["AppointmentService:BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilience();

    builder.Services.AddHttpClient<IQueryServiceClient, QueryServiceClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["QueryService:BaseUrl"]!))
        .AddHttpMessageHandler<AuthTokenHandler>()
        .AddStandardResilience();

    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddStackExchangeRedisCache(o =>
    o.Configuration = builder.Configuration["Redis:ConnectionString"]);

    QuestPDF.Settings.License = LicenseType.Community;

    var jwtSettings = new JwtSettings
    {
        Key = builder.Configuration["Jwt:Key"]!,
        Issuer = builder.Configuration["Jwt:Issuer"]!,
        Audience = builder.Configuration["Jwt:Audience"]!,
        ExpiryMinutes = int.Parse(builder.Configuration["Jwt:ExpiryMinutes"]!)
    };

    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    ErrorCode = "UNAUTHORIZED",
                    Message = "You must be logged in to access this resource"
                });
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser().Build();
    });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters
                .Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter your JWT token here"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddFrontendCors(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowFrontend");

    app.UseHttpsRedirection();
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