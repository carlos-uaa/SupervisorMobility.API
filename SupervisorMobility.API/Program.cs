using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using SupervisorMobility.API;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/Supervisor.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("Cors", builder =>
        builder.WithOrigins("*")
        .AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
        .SetIsOriginAllowedToAllowWildcardSubdomains().WithExposedHeaders("*")
 );
});

var env = builder.Environment;
if (env.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: false);
}
else
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}

builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
 .AddXmlDataContractSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
    {
        return type.FullName;
    });
});

builder.Services.RegisterBusinessServices();
builder.Services.RegisterDataServices(builder.Configuration);

//Odmitir Referencias ciruclares
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 1073741824;
});

var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();

builder.Services.AddSingleton(emailConfig);

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    // Capturar la respuesta después de que se haya completado
    await next();

    // Registrar los headers de la respuesta
    foreach (var header in context.Response.Headers)
    {
        Console.WriteLine($"Response Header: {header.Key} = {string.Join(",", header.Value)}");
        Debug.WriteLine($"Response Header: {header.Key} = {string.Join(",", header.Value)}");
    }
});

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("Cors");
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();

public class MyJob : IJob
{
    private readonly IAssyChartService _assyChartService;
    private readonly ISupervisorMobilityRepository _supervisorMobilityService;
    private readonly IEmailService _emailService;

    public MyJob(IAssyChartService assyChartService, ISupervisorMobilityRepository supervisorMobilityService, IEmailService emailService)
    {
        _assyChartService = assyChartService;
        _supervisorMobilityService = supervisorMobilityService;
        _emailService = emailService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Job executed !!!!!");
        Console.ResetColor();

        var _allJobObservations = await _supervisorMobilityService.GetAllJobObservationsAsync(includePeople: true, includeLup: true);
        if (_allJobObservations != null)
        {
            var filteredJobObservations = _allJobObservations
                .Where(j => j.Lup.Any(l => l.IsActive == true && (l.Status == 1 || l.Status == 2)))
                .ToList();

            var supervisorLupCounts = filteredJobObservations
             .GroupBy(j => new { j.SupervisorId, j.Supervisor?.Name, j.Supervisor?.SuperiorId, j.Supervisor?.Email })
             .Select(g => new
             {
                 SupervisorId = g.Key.SupervisorId,
                 SupervisorName = g.Key.Name,
                 SuperiorId = g.Key.SuperiorId,
                 SupervisorEmail = g.Key.Email,
                 ActiveLupCount = g.Sum(j => j.Lup.Count(l => l.IsActive == true && (l.Status == 1 || l.Status == 2)))
             })
             .ToList();

            foreach (var supervisor in supervisorLupCounts)
            {
                string notificationText = supervisor.ActiveLupCount == 1
                                    ? $"Supervisor {supervisor.SupervisorName} has 1 LUP item active at {DateTime.Now:hh:mm tt}"
                                    : $"Supervisor {supervisor.SupervisorName} has {supervisor.ActiveLupCount} LUP items active at {DateTime.Now:hh:mm tt}";

                NotificationToCreateDto newnotify = new NotificationToCreateDto
                {
                    MadeBy = "SM Mobility",
                    UserId = supervisor.SupervisorId.Value,
                    IsAccepted = true,
                    IsActive = true,
                    NotificationText = notificationText,
                    NotificationType = "Active Lup Item"
                };

                var response = await _assyChartService.CreateNotificationAsync(newnotify);

                if (response != null)
                {
                    var emailMessageError = _emailService.CreateEmailMessage(supervisor.SupervisorEmail, "Active Lup Item", notificationText);
                    //var emailMessageError = _emailService.CreateEmailMessage("pmunoz@gruposinco.com.mx", "Active Lup Item", notificationText);
                    _emailService.Send(emailMessageError);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Notification created for Supervisor {supervisor.SupervisorName}");
                    Console.ResetColor();
                }

                if (supervisor.SuperiorId.HasValue)
                {
                    NotificationToCreateDto newnotifyForSSV = new NotificationToCreateDto
                    {
                        MadeBy = "SM Mobility",
                        UserId = supervisor.SuperiorId.Value,
                        IsAccepted = true,
                        IsActive = true,
                        NotificationText = notificationText,
                        NotificationType = "Active Lup Item"
                    };

                    var responseForSSV = await _assyChartService.CreateNotificationAsync(newnotifyForSSV);

                    if (responseForSSV != null)
                    {

                        User SSV = await _supervisorMobilityService.GetUserAsync(supervisor.SuperiorId.Value);
                        if(SSV != null)
                        {
                            var emailMessageError = _emailService.CreateEmailMessage(SSV.Email, "Active Lup Item", notificationText);
                            //var emailMessageError = _emailService.CreateEmailMessage("pmunoz@gruposinco.com.mx", "Active Lup Item SSV", notificationText);
                            _emailService.Send(emailMessageError);
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Notification created for Senior Supervisor of {supervisor.SupervisorName}");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
