using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.Services;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using SupervisorMobility.API.Models.NotificationDtos;


namespace SupervisorMobility.API
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection RegisterBusinessServices(
           this IServiceCollection services)
        {
            services.AddScoped<IJobObservationService, JobObservationService>();
            services.AddScoped<IAssyChartService, AssyChartService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITreeService, TreeService>();


            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 1073741824;
                options.MaxRequestBodyBufferSize = 1073741824;
            });

            services.AddMvc().AddNewtonsoftJson();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet " });
            });

            return services;
        }

        public static IServiceCollection RegisterDataServices(
            this IServiceCollection services, IConfiguration configuration, IWebHostBuilder hostBuilder)
        {
            // add the DbContext
            services.AddDbContext<SupervisorMobilityContext>(options => options.UseSqlServer(configuration.GetConnectionString("SupervisorMobilityDBConnectionString")), ServiceLifetime.Transient);

            //Add automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // register the repository
            services.AddScoped<ISupervisorMobilityRepository, SupervisorMobilityRepository>();
            // register Stamping the repository
            services.AddScoped<IStampingRepository, StampingRepository>();

            //custos HTTP client service
            services.AddScoped<CustomHttpClientService>();

            // servicio de ejecucion en segundo plano
            // Procesamiento de documento Headcount
            // Procesamiento de documento Plant Strcuture Building (Carga de trabajo)
            services.AddSingleton<BackgroundProcessingService>();

            //Lanel Attendance Service
            services.AddHostedService<LanelAttendanceService>();

            var emailConfig = configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            services.AddSingleton(emailConfig);

            //Odmitir Referencias ciruclares
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            // Configurar Quartz.NET
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                var jobKey = new JobKey("MyJob");
                q.AddJob<MyJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("MyJob-trigger")
                    .WithCronSchedule("0 26 9 * * ?"));
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


            //configuracion del tamaño de archivos 
            hostBuilder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 1073741824;
            });

            return services;
        }

      
    }
}


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
                        if (SSV != null)
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
