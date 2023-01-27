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
builder.Configuration.AddJsonFile("appsettings.json", optional:false, reloadOnChange: true);

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
<<<<<<< Updated upstream
builder.Services.AddCors(policy => {

    policy.AddPolicy("Policy_Name", builder =>
      builder.WithOrigins("https://*:7017/")
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
 );
});


=======
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("https://localhost:44398").AllowAnyMethod().AllowAnyHeader();
}));
>>>>>>> Stashed changes

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("Policy_Name");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("corspolicy");

app.MapControllers();

app.Run();
