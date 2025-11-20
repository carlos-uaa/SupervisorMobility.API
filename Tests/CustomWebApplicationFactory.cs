using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupervisorMobility.API.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Elimina el DbContext real
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SupervisorMobilityContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Agrega el DbContext en memoria
                services.AddDbContext<SupervisorMobilityContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Construye el proveedor de servicios
                var sp = services.BuildServiceProvider();

                // Crea el scope y precarga datos
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SupervisorMobilityContext>();

                db.Database.EnsureCreated();


            });
        }
    }
}
