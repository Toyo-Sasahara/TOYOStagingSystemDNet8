using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.WindowsServices;
using NetworkPrintService;
using SasaLib;
using Serilog;
using Serilog.Core;
using ToyoStageService;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "TOYONETPRINTSVC";
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<PrintCoordinator>();

var app = builder.Build();

InitializeServerLogIfNeeded();

app.MapGet("/healthz", (ILogger<Program> logger) =>
{
    logger.LogInformation("GET /healthz request received.");
    return Results.Ok(new { status = "ok" });
});

app.MapPost("/print", async ([FromForm] PrintRequestForm request, PrintCoordinator coordinator, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    logger.LogInformation("POST /print request received.");

    var result = await coordinator.PrintAsync(request, cancellationToken);

    if (result.IsSuccess)
    {
        return Results.Accepted(value: new
        {
            message = result.Message,
            detail = result.Detail,
        });
    }

    return Results.Json(
        new
        {
            message = result.Message,
            detail = result.Detail,
        },
        statusCode: result.StatusCode);
})
.DisableAntiforgery();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

static void InitializeServerLogIfNeeded()
{
    var logFolder = Path.Combine(AppContext.BaseDirectory, "logs");
    Directory.CreateDirectory(logFolder);

    ServerLog.MainLogging ??= new Logging(logFolder, "NPS-Main.log");
    ServerLog.OtherLogging ??= new Logging(logFolder, "NPS-Other.log");
    ServerLog.PipeLogging ??= new Logging(logFolder, "NPS-Pipe.log");
    ServerLog.DebugLogging ??= new Logging(logFolder, "NPS-Debug.log");
}
