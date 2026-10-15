using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


// ContentRootPath = the exe folder, because Claude Code starts this
// process from its own working directory
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// stdout belongs to the MCP protocol, so every log line goes to stderr
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

foreach (var (name, key) in new[]
{
    (ApiNames.Auth, "Auth"), (ApiNames.Command, "Command"),
    (ApiNames.Query, "Query"), (ApiNames.Appointments, "Appointments")
})
{
    var url = builder.Configuration[$"Services:{key}"]
        ?? throw new InvalidOperationException($"Services:{key} is not configured.");
    builder.Services.AddHttpClient(name, c => c.BaseAddress = new Uri(url));
}
