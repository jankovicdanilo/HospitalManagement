using HospitalManagement.Data;
using HospitalManagement.Middleware;
using HospitalManagement.Repositories.Implementations;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Implementations;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using FluentValidation;



// tell ASP.NET Core to use Serilog
var builder = WebApplication.CreateBuilder(args);

// configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File
    (
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7 // keep last 7 days of logs
    ).CreateLogger();

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// Register HospitalDbContext with SQL Server using connection string from appsettings.json
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalDb")));

builder.Services.AddScoped<IAuthRepository,AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorService, DoctorService>();

// Read JWT settings for token validation configuration
var jwtSettings = new JwtSettings
{
    Key = builder.Configuration["Jwt:Key"]!,
    Issuer = builder.Configuration["Jwt:Issuer"]!,
    Audience = builder.Configuration["Jwt:Audience"]!,
    ExpiryMinutes = int.Parse(builder.Configuration["Jwt:ExpiryMinutes"]!)
};

// Bind Jwt section from appsettings.json to JwtSettings class for DI
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// Configure JWT authentication as the default scheme
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
            ValidateIssuer = true,           // check token issuer matches
            ValidateAudience = true,         // check token audience matches
            ValidateLifetime = true,         // check token hasn't expired
            ValidateIssuerSigningKey = true, // check token signature is valid
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            // signing key used to verify the token signature
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

// Enable authorization middleware
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser().Build();
});

// Register controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Required for Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support so protected endpoints can be tested directly in Swagger UI
builder.Services.AddSwaggerGen(options =>
{
    // Define Bearer token security scheme
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token here"
    });

    // Apply the security scheme globally to all endpoints
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

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
    await SeedData.SeedAdminAsync(context);
}

// Enable Swagger only in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// UseAuthentication must come before UseAuthorization - order matters
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();