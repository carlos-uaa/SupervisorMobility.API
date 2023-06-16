using System.Diagnostics;

namespace SupervisorMobility.API
{
    public class ScheduledTask : BackgroundService
    {
        private readonly ILogger<ScheduledTask> _logger;

        public ScheduledTask(ILogger<ScheduledTask> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               
                var currentTime = DateTime.Now;

               
                if (currentTime.Hour == 8 && currentTime.Minute == 0)
                {
                    
                    _logger.LogInformation("Tarea programada ejecutada a las 8:00 AM");
                    
                }


                _logger.LogInformation("Tarea programada ejecutada a las 8:00 AM");
                Debug.WriteLine("Tarea programada");

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
