using Microsoft.Extensions.Hosting;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class NotificationProcessorHostedService : BackgroundService
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationProcessorHostedService> _logger;

        public NotificationProcessorHostedService(
            INotificationService notificationService,
            ILogger<NotificationProcessorHostedService> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Notification Processor Background Service");

            if (_notificationService is NotificationService service)
            {
                await service.StartProcessingAsync(stoppingToken);
                
                // Mantém o serviço rodando
                try
                {
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Notification Processor is stopping");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Notification Processor Background Service");

            if (_notificationService is NotificationService service)
            {
                await service.StopProcessingAsync(cancellationToken);
            }

            await base.StopAsync(cancellationToken);
        }
    }
}