using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Services.EmailService;

namespace SupervisorMobility.API.Services.BackgroundServices
{
    public class EmailQueueBackgroundService : BackgroundService
    {


        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailQueueBackgroundService> _logger;
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        private int _isProcessing = 0;
        private int _pendingSignals = 0; // Contador de señales pendientes


        public EmailQueueBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<EmailQueueBackgroundService> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }



        public void TriggerProcessing()
        {
            var isCurrentlyProcessing = Interlocked.CompareExchange(ref _isProcessing, 0, 0) == 1;
            
            if (isCurrentlyProcessing)
            {
                // Incrementar contador de señales pendientes
                Interlocked.Increment(ref _pendingSignals);
                _logger.LogInformation("Procesamiento en curso. Señal encolada para reprocesar después.");
            }
            else
            {
                _signal.Release();
                _logger.LogInformation("Señal de procesamiento enviada");
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Queue Background Service iniciado en modo event-driven");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(stoppingToken);

                    Interlocked.Exchange(ref _isProcessing, 1);

                    await ProcessPendingEmails(stoppingToken);

                    Interlocked.Exchange(ref _isProcessing, 0);

                    // Verificar si hubo señales mientras procesaba
                    if (Interlocked.CompareExchange(ref _pendingSignals, 0, 0) > 0)
                    {
                        _logger.LogInformation($"Había {_pendingSignals} señal(es) pendiente(s). Reprocesando...");
                        Interlocked.Exchange(ref _pendingSignals, 0);
                        _signal.Release(); // Volver a procesar
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Procesamiento cancelado");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en Email Queue Background Service");
                    Interlocked.Exchange(ref _isProcessing, 0);
                }
            }

            _logger.LogInformation("Email Queue Background Service detenido");
        }

        private async Task ProcessPendingEmails(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var emailQueueService = scope.ServiceProvider.GetRequiredService<IEmailQueueService>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    var pendingQueuesResponse = await emailQueueService.GetPendingEmailQueuesAsync();

                    if (pendingQueuesResponse.Success && pendingQueuesResponse.Data != null && pendingQueuesResponse.Data.Count > 0)
                    {
                        _logger.LogInformation($"Procesando {pendingQueuesResponse.Data.Count} email(s) pendiente(s)");

                        foreach (var emailQueue in pendingQueuesResponse.Data)
                        {
                            try
                            {
                                var emailService = scope.ServiceProvider.GetRequiredService<IEmailServices>();
                                var wasSent =  await emailService.SendEmailAsync(emailQueue);
                                if(wasSent == false)
                                {
                                    await emailQueueService.IncrementAttempt(emailQueue.EmailQueueID);
                                }
                                else
                                {
                                    await emailQueueService.AcceptEmailQueueAsync(emailQueue.EmailQueueID);
                                }
                                    _logger.LogInformation($"Email procesado: EmailQueue ID: {emailQueue.EmailQueueID}");
                                
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error procesando EmailQueue ID: {emailQueue.EmailQueueID}");
                            }
                        }

                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    }
                    else
                    {
                        _logger.LogInformation("No hay más emails pendientes. Servicio en espera.");
                        break;
                    }
                }
            }
        }
    }
}
