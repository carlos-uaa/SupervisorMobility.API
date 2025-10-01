// - Core .NET imports
using System.Runtime.InteropServices;

// - Microsoft imports
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

// - External imports
using Quartz;
using AutoMapper;
using Quartz.Spi;
using Quartz.Impl;
using Newtonsoft.Json;

// - Context imports
using SupervisorMobility.API.Context;

// - Entity imports
using SupervisorMobility.API.DataAccess.Entities;

// - Data access / Service imports
using SupervisorMobility.API.DataAccess.Services.OrderingServices;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;

// - Business / Service imports
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Services.SOS;

// - Interface imports
using SupervisorMobility.API.Interfaces.SOS;

// - Model / DTO imports
using SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Knowledges;
using SupervisorMobility.API.Interfaces.SOSDistribution.SOSDistributionExcel;
using SupervisorMobility.API.Services.SOSDistribution.SOSDistributionExcel;
using SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Skills;
using SupervisorMobility.API.infrastructure.repositories.STRO;
using SupervisorMobility.API.Models.NotificationDtos;


namespace SupervisorMobility.API
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection RegisterBusinessServices(
           this IServiceCollection services)
        {
            //services.AddSingleton<ContextFactory>();
            // register the repository
            services.AddScoped<ISupervisorMobilityRepository, SupervisorMobilityRepository>();
            // register Stamping the repository
            services.AddScoped<IStampingRepository, StampingRepository>();
            // HOE/SOS Analysis_Process
            services.AddScoped<ISOS_ProcessRepository, SOS_ProcessRepository>();
            services.AddScoped<ISOSDistributionExcelService, SOSDistributionExcelService>();

            services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<ISTROSequencesRepository, STROSequencesRepository>();
            //services.AddSingleton<ISOSAnalysis_ProcessRepository, SOSAnalysis_ProcessRepository>(sp =>
            //{
            //    var scopeFactory = sp.GetRequiredService<ContextFactory>();
            //    var mapper = sp.GetRequiredService<IMapper>();
            //    return new SOSAnalysis_ProcessRepository(scopeFactory, mapper);
            //});

            //Another
            services.AddScoped<IJobObservationService, JobObservationService>();
            services.AddScoped<IAssyChartService, AssyChartService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITreeService, TreeService>();
            services.AddScoped<IOrderingService, OrderingService>();
            services.AddScoped<ISTOperatingRequirementsService, STOperatingRequirementsService>();
            services.AddScoped<ISTROSyncDistributionService, STROSyncDistributionService>();



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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Usa la cadena de conexión de Windows
                services.AddDbContext<SupervisorMobilityContext>(options => options.UseSqlServer(configuration.GetConnectionString("SupervisorMobilityDBConnectionString"), options => { options.CommandTimeout(300); }), ServiceLifetime.Transient);
            }
            else
            {
                // Usa la cadena de conexión de Linux
                services.AddDbContext<SupervisorMobilityContext>(options => options.UseSqlServer(configuration.GetConnectionString("SupervisorMobilityDBConnectionLinuxString"), options => { options.CommandTimeout(300); }), ServiceLifetime.Transient);
            }
            //Add automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



            //custos HTTP client service
            services.AddScoped<CustomHttpClientService>();

            // servicio de ejecucion en segundo plano
            // Procesamiento de documento Headcount
            // Procesamiento de documento Plant Strcuture Building (Carga de trabajo)
            services.AddSingleton<BackgroundProcessingService>();

            //Lanel Attendance Service
            // services.AddHostedService<LanelAttendanceService>();

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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Configurar Quartz.NET
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();

                    var jobKey = new JobKey("ActiveLupItemsJob");
                    q.AddJob<ActiveLupItemsJob>(opts => opts.WithIdentity(jobKey));
                    q.AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .WithIdentity("ActiveLupItemsJob-trigger")
                        .WithCronSchedule("0 0 7 * * ?"));
                });

                services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            }

            //configuracion del tamaño de archivos 
            hostBuilder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 1073741824;
            });

            return services;
        }


    }
}

