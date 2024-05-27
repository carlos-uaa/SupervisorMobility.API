using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.Services;

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
            this IServiceCollection services, IConfiguration configuration)
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


            return services;
        }

      
    }
}
