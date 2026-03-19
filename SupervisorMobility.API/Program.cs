using Serilog;
using SupervisorMobility.API;

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

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
 .AddXmlDataContractSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
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
    app.UseSwaggerUI(c =>
    {
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // contrae todo
    });

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

public partial class Program { }
