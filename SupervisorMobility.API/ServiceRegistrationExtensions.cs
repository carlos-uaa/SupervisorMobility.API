using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
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

            ////this
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 1073741824;
            });
            ////this


            return services;
        }

        public static IServiceCollection RegisterDataServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // add the DbContext
            services.AddDbContext<SupervisorMobilityContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SupervisorMobilityDBConnectionString")));

            // register the repository
            services.AddScoped<ISupervisorMobilityRepository, SupervisorMobilityRepository>();
            return services;
        }
    }
}
