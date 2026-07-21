using AutoMapper;
using HospitalManagement.InvoiceService.Clients.Implementations;
using HospitalManagement.InvoiceService.Clients.Interfaces;
using HospitalManagement.InvoiceService.Services.Docx;
using HospitalManagement.InvoiceService.Services.Implementations;
using HospitalManagement.InvoiceService.Services.Interfaces;
using HospitalManagement.InvoiceService.Services.Pdf;
using HospitalManagement.Shared.Extensions;
using HospitalManagement.Shared.Http;
using HospitalManagement.Shared.Settings;
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

    // Services
    builder.Services.AddScoped<IBillingService, BillingService>();
    builder.Services.AddScoped<IInvoiceDocumentGenerator, PdfInvoiceGenerator>();
    builder.Services.AddScoped<IInvoiceDocumentGenerator, DocxInvoiceGenerator>();
    builder.Services.AddScoped<IInvoiceDocumentGeneratorFactory, InvoiceDocumentGeneratorFactory>();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<AuthTokenHandler>();

    // HTTP client for cross-service calls to Appointments
    builder.Services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["AppointmentService:BaseUrl"]!);
    })
        .AddHttpMessageHandler<AuthTokenHandler>();

    builder.Services.AddAutoMapper(typeof(Program));

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