using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SupervisorMobility.API;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
// Verifica si el sistema operativo es Linux
bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

if (env.IsDevelopment())
{
    // Usa una ruta completa si es Linux
    if (isLinux)
    {
        builder.Configuration.AddJsonFile("/home/Vanitas/Documents/GrupoSinco/Supervisor Mobility/SupervisorMobility.API/SupervisorMobility.API/appsettings.Development.json", optional: false, reloadOnChange: false);
    }
    else
    {
        builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: false);
    }
}
else
{
    // Usa una ruta completa si es Linux
    if (isLinux)
    {
        builder.Configuration.AddJsonFile("/home/Vanitas/Documents/GrupoSinco/Supervisor Mobility/SupervisorMobility.API/SupervisorMobility.API/appsettings.json", optional: false, reloadOnChange: true);
    }
    else
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    }
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
builder.Services.RegisterDataServices(builder.Configuration, builder.WebHost);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.Use(async (context, next) =>
//{
//    // Capturar la respuesta después de que se haya completado
//    await next();

//    // Registrar los headers de la respuesta
//    foreach (var header in context.Response.Headers)
//    {
//        Console.WriteLine($"Response Header: {header.Key} = {string.Join(",", header.Value)}");
//        Debug.WriteLine($"Response Header: {header.Key} = {string.Join(",", header.Value)}");
//    }
//});

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("Cors");
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
