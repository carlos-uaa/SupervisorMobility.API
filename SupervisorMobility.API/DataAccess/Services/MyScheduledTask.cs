using AutoMapper;
using CsvHelper;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AttendanceDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;
using System.Globalization;

namespace SupervisorMobility.API.DataAccess.Services
{

    public class MyScheduledTask : BackgroundService
    {
        private readonly ILogger<MyScheduledTask> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
     

        public MyScheduledTask(IServiceScopeFactory serviceScopeFactory, ILogger<MyScheduledTask> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;         
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Obtener el scope del servicio
                _logger.LogInformation("La tarea programada se Inicio...");


            // Implementar tu lógica para la ejecución en segundo plano
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.Now.TimeOfDay;
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;

                    var _assyChartService = serviceProvider.GetService<IAssyChartService>();
                    var _mapper = serviceProvider.GetService<IMapper>();

                    if (IsWithinTimeInterval(currentTime, TimeSpan.FromHours(6.25), TimeSpan.FromHours(9.25)))
                    {
                        _logger.LogInformation("La tarea programada se está ejecutando De 6:15 am a 9:15 am...");
                        Execute(_assyChartService, _mapper);
                        await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Espera 30 minutos antes de la siguiente ejecución
                    }
                    else if (IsWithinTimeInterval(currentTime, TimeSpan.FromHours(10), TimeSpan.FromHours(22)))
                    {
                        _logger.LogInformation("La tarea programada se está ejecutando De 10:00 am a 10:00 pm");
                        Execute(_assyChartService, _mapper);
                        await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
                    }
                    else if (IsWithinTimeInterval(currentTime, TimeSpan.FromHours(22.25), TimeSpan.FromHours(25.25)))
                    {
                        _logger.LogInformation("La tarea programada se está ejecutando De De 10:15 pm a 1:15 am ...");
                        Execute(_assyChartService, _mapper);
                        await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Espera 30 minutos antes de la siguiente ejecución
                    }
                    else if (IsWithinTimeInterval(currentTime, TimeSpan.FromHours(2), TimeSpan.FromHours(6)))
                    {
                        _logger.LogInformation("La tarea programada se está ejecutandoDe 2:00 am a 6 am...");
                        Execute(_assyChartService, _mapper);
                        await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
                    }
                    else
                    {
                        _logger.LogInformation("La tarea programada se está ejecutando...");
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Espera 1 minutos antes de la siguiente ejecución
                    }
                }
               
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            // Implementar la lógica de limpieza o liberación de recursos al detener el servicio
            // ...
            _logger.LogInformation("La tarea programada se detuvo...");

            await base.StopAsync(stoppingToken);
        }

        private bool IsWithinTimeInterval(TimeSpan currentTime, TimeSpan startTime, TimeSpan endTime)
        {
            // Verifica si la hora actual está dentro del intervalo de tiempo especificado
            return currentTime >= startTime && currentTime <= endTime;
        }

        public async void Execute(IAssyChartService _assyChartService, IMapper _mapper)
        {
            string filePath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\attendance\\attendance.csv";
            //string filePath = @"C:\LenelInfo\attendance.csv";


            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>();

            List<Attendance> allattendance = _mapper.Map<List<Attendance>>(await _assyChartService.GetAllAttendanceAsync());
            List<UsersWithoutNavigationDetails> alluser = _mapper.Map<List<UsersWithoutNavigationDetails>>(await _assyChartService.GetAllUsers());
            var fecha2 = DateTime.Now;

            List<Attendance> allattendanceadded = new List<Attendance>();


            foreach (var record in records)
            {
                int id = 0;

                if ((string)record.Id_Empleado != "")
                    id = int.Parse(record.Id_Empleado);
                else
                    continue;


                DateTime fecha = DateTime.Now;

                if ((string)record.Fecha != "")
                    fecha = DateTime.Parse((string)record.Fecha);
                else
                    continue;


                var user = alluser.Find(u => u.Payroll == id);
                if (user == null)
                {
                    continue;
                }

                var existingRecordInAttendance = allattendance.Find(a => a.User.Payroll == id);
                bool mismoDia = fecha.Day == fecha2.Day && fecha.Month == fecha2.Month && fecha.Year == fecha2.Year;

                if (!mismoDia)
                {
                    //es un dia diferente
                    if (existingRecordInAttendance != null)
                    {
                        //ya se encuentra en la lista
                        var updateRecord = new AttendanceForUpdateDto
                        {
                            UserId = user.UserId,
                            SuperiorId = user.SuperiorId,
                            CurrentdistributionId = user.DistributionId,
                            Compas = false,
                            Station = false
                        };

                        bool update = await _assyChartService.UpdateAttendanceAsync(updateRecord, existingRecordInAttendance);
                        //se actualiza dado que es de un dia pasado y no estara en la planta 
                    }
                    else
                    {
                        //El registro no existe y es un dia diferente

                        var newRecorddifday = new AttendanceForCreationDto
                        {
                            UserId = user.UserId,
                            SuperiorId = user.SuperiorId,
                            CurrentdistributionId = user.DistributionId,
                            Compas = mismoDia,
                            Station = false
                        };


                        var processAttendanceYesterday = await _assyChartService.CreateAttendanceAsync(newRecorddifday);
                        allattendanceadded.Add(processAttendanceYesterday);
                    }

                    continue;

                }
                else
                {
                    //Es el mismo dia
                    DateTime inico = DateTime.Now;
                    DateTime fin = DateTime.Now;




                    //el inicio puede estar vacio por que es turno vespertino u horas extras
                    if ((string)record.Inicio == "")
                    {
                        //ya existe registro de el en tabla de asistencia
                        if ((string)record.Fin != "")
                        {
                            //Update, ya salio
                            var updateRecord = new AttendanceForUpdateDto
                            {
                                UserId = user.UserId,
                                SuperiorId = user.SuperiorId,
                                CurrentdistributionId = user.DistributionId,
                                Compas = false,
                                Station = false
                            };

                            bool update = await _assyChartService.UpdateAttendanceAsync(updateRecord, existingRecordInAttendance);
                            continue;
                        }
                    }
                    else
                    {
                        if ((string)record.Fin == "")
                        {
                            //aun no sale, no se hace nada
                            if (existingRecordInAttendance != null)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            //Update, ya salio
                            var updateRecord = new AttendanceForUpdateDto
                            {
                                UserId = user.UserId,
                                SuperiorId = user.SuperiorId,
                                CurrentdistributionId = user.DistributionId,
                                Compas = false,
                                Station = false
                            };

                            bool update = await _assyChartService.UpdateAttendanceAsync(updateRecord, existingRecordInAttendance);
                            continue;

                        }
                    }

                    if (existingRecordInAttendance != null)
                    {
                        continue;
                    }
                }


                var newRecord = new AttendanceForCreationDto
                {
                    UserId = user.UserId,
                    SuperiorId = user.SuperiorId,
                    CurrentdistributionId = user.DistributionId,
                    Compas = mismoDia,
                    Station = false
                };


                var processAttendance = await _assyChartService.CreateAttendanceAsync(newRecord);
                allattendanceadded.Add(processAttendance);

            }

        }
    }





}
