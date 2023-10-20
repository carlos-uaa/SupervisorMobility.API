using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AttendanceDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.DataAccess.Services
{
    public class MyScheduledTaskService : BackgroundService
    {

        private readonly ILogger<MyScheduledTaskService> _logger;
        private readonly HttpClient _httpClient;
        
        public MyScheduledTaskService(ILogger<MyScheduledTaskService> logger, CustomHttpClientService customHttp)
        {
            _httpClient = customHttp.GetApiHttpClient();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Aquí colocas la lógica de tu tarea programada
                var currentTime = DateTime.Now.TimeOfDay;
                // Loguea un mensaje cada vez que se ejecuta la tarea
                _logger.LogInformation("La tarea programada se está ejecutando...");


                if (IsWithinTimeInterval(currentTime, TimeSpan.FromHours(6.25), TimeSpan.FromHours(9.25)))
                {
                    _logger.LogInformation("La tarea programada se está ejecutando De 6:15 am a 9:15 am...");
                    Execute();
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Espera 30 minutos antes de la siguiente ejecución
                }
                else if (IsWithinTimeInterval(currentTime, TimeSpan.FromHours(10), TimeSpan.FromHours(22)))
                {
                    _logger.LogInformation("La tarea programada se está ejecutando De 10:00 am a 10:00 pm");
                    Execute();
                    await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
                }
                else if (IsWithinTimeInterval(currentTime, TimeSpan.FromHours(22.25), TimeSpan.FromHours(25.25)))
                {
                    _logger.LogInformation("La tarea programada se está ejecutando De De 10:15 pm a 1:15 am ...");
                    Execute();
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Espera 30 minutos antes de la siguiente ejecución
                }
                else if (IsWithinTimeInterval(currentTime, TimeSpan.FromHours(2), TimeSpan.FromHours(6)))
                {
                    _logger.LogInformation("La tarea programada se está ejecutandoDe 2:00 am a 6 am...");
                    Execute();
                    await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
                }
                else
                {
                    _logger.LogInformation("La tarea se esta ejecutando sin ningun parametro de tiempo...");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Espera 1 minutos antes de la siguiente ejecución
                }

            }
        }

        private bool IsWithinTimeInterval(TimeSpan currentTime, TimeSpan startTime, TimeSpan endTime)
        {
            // Verifica si la hora actual está dentro del intervalo de tiempo especificado
            return currentTime >= startTime && currentTime <= endTime;
        }

        public async void Execute()
        {
            try
            {
                // Realizar la llamada al endpoint deseado dentro de la misma API
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Attendance/Assign");

                // Verificar si la respuesta es exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Procesar la respuesta exitosa
                    string responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Tare Programada Satus OK...");

                }
                else
                {
                    // Manejar el error de la respuesta
                    _logger.LogInformation($"Tare Programada Satus {(int)response.StatusCode}...");

                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra durante la llamada
                _logger.LogInformation($"Tare Programada Satus 500 ...");

            }

        }
    }
}
