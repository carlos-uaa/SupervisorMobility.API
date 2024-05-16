using Newtonsoft.Json;
using Serilog;
using SupervisorMobility.API;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services;
using System.Diagnostics;

//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/Supervisor.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//Add Cors
builder.Services.AddCors(policy => {

    policy.AddPolicy("Cors", builder =>
        builder.WithOrigins("*")
        .AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
        .SetIsOriginAllowedToAllowWildcardSubdomains().WithExposedHeaders("*")
 );
});


//add json file to builder configuration
var env = builder.Environment;
if (env.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: false);
}
else
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}

// Add services to the container.
// Configure the HTTP request pipeline.

builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
    {
        
        return type.FullName;
    });
});

//Add other services
builder.Services.RegisterBusinessServices();
builder.Services.RegisterDataServices(builder.Configuration);

//


//Add automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

//mail
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);


//peticion api
//builder.Services.AddHostedService<MyScheduledTaskService>();


//using namespaces este funciona mejor 
//builder.Services.AddHostedService<MyScheduledTask>();


// Crear una instancia del servicio en segundo plano
builder.Services.AddSingleton<BackgroundProcessingService>();


builder.Services.AddScoped<CustomHttpClientService>();



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