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
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;


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


            //Aca un ejemplo de servicio

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

                var jobKey = new JobKey("ActiveLupItemsJob");
                q.AddJob<ActiveLupItemsJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("ActiveLupItemsJob-trigger")
                    .WithCronSchedule("0 0 7 * * ?"));
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

