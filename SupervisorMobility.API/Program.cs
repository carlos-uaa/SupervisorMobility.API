using Microsoft.AspNetCore.Hosting;
using Serilog;
using SupervisorMobility.API;

//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//add json file to builder configuration
var env = builder.Environment;
if (env.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}
// Add services to the container.
 
builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add other services
builder.Services.RegisterBusinessServices();
builder.Services.RegisterDataServices(builder.Configuration);

//Add automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Add Cors
builder.Services.AddCors(policy => {

    policy.AddPolicy("Cors", builder =>
      builder.WithOrigins("https://*:10201/")
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
 );

});


builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize= 1073741824;
});

var app = builder.Build();



// Configure the HTTP request pipeline.
app.UseCors("Cors");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
AppContext.SetSwitch("Switch.Microsoft.AspNetCore.Mvc.EnableRangeProcessing",true);