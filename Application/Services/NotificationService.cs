using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Application.Services
{
    public class NotificationService : INotificationService, IAsyncDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration,
            ILogger<NotificationService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory
                ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configuração do Service Bus
            string connectionString = configuration["ServiceBus:ConnectionString"]
                ?? throw new InvalidOperationException("ServiceBus ConnectionString not configured");

            string queueName = configuration["ServiceBus:NotificationQueueName"]
                ?? throw new InvalidOperationException("ServiceBus QueueName not configured");

            _client = new ServiceBusClient(connectionString);

            var processorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 5,
                AutoCompleteMessages = false,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
            };

            _processor = _client.CreateProcessor(queueName, processorOptions);

            // Registra os handlers
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
        }

        /// <summary>
        /// Inicia o processamento de mensagens do Service Bus
        /// </summary>
        public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            await _processor.StartProcessingAsync(cancellationToken);
            _logger.LogInformation("Notification Service Bus Processor started");
        }

        /// <summary>
        /// Para o processamento de mensagens do Service Bus
        /// </summary>
        public async Task StopProcessingAsync(CancellationToken cancellationToken = default)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            _logger.LogInformation("Notification Service Bus Processor stopped");
        }

        /// <summary>
        /// Handler que processa as mensagens recebidas do Service Bus
        /// </summary>
        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            // Cria um scope para resolver dependências Scoped
            using var scope = _serviceScopeFactory.CreateScope();
            var notificationRepository = scope.ServiceProvider
                .GetRequiredService<INotificationRepository>();

            try
            {
                string messageBody = args.Message.Body.ToString();
                _logger.LogInformation($"Notification message received: {args.Message.MessageId}");

                // Deserializa a notificação
                var notification = JsonSerializer.Deserialize<Notification>(messageBody);

                if (notification == null)
                {
                    _logger.LogWarning($"Failed to deserialize notification message: {args.Message.MessageId}");
                    await args.DeadLetterMessageAsync(args.Message, "DeserializationError", "Could not deserialize message body");
                    return;
                }

                // Registra a notificação no banco de dados usando o repository do scope
                await notificationRepository.RegisterNotificationAsync(notification);

                // Completa a mensagem (remove da fila)
                await args.CompleteMessageAsync(args.Message);

                _logger.LogInformation($"Notification processed successfully: UserId={notification.UserId}, Type={notification.Title}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing notification message: {args.Message.MessageId}");

                // Verifica o número de tentativas
                if (args.Message.DeliveryCount >= 3)
                {
                    // Move para Dead Letter Queue após 3 tentativas
                    await args.DeadLetterMessageAsync(
                        args.Message,
                        "MaxDeliveryCountExceeded",
                        $"Message failed after {args.Message.DeliveryCount} attempts: {ex.Message}"
                    );
                    _logger.LogError($"Message moved to DLQ: {args.Message.MessageId}");
                }
                else
                {
                    // Abandona a mensagem (volta para a fila para reprocessamento)
                    await args.AbandonMessageAsync(args.Message);
                    _logger.LogWarning($"Message abandoned for retry: {args.Message.MessageId}");
                }
            }
        }

        /// <summary>
        /// Handler para erros gerais do processador
        /// </summary>
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception,
                $"Service Bus error - Source: {args.ErrorSource}, Entity: {args.EntityPath}");
            return Task.CompletedTask;
        }

        public async Task RegisterNotificationAsync(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            // Cria um scope para usar o repository
            using var scope = _serviceScopeFactory.CreateScope();
            var notificationRepository = scope.ServiceProvider
                .GetRequiredService<INotificationRepository>();

            try
            {
                await notificationRepository.RegisterNotificationAsync(notification);
                _logger.LogInformation($"Notification registered in database: {notification.NotificationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering notification: {notification.NotificationId}");
                throw;
            }
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null or empty", nameof(userId));

            // Cria um scope para usar o repository
            using var scope = _serviceScopeFactory.CreateScope();
            var notificationRepository = scope.ServiceProvider
                .GetRequiredService<INotificationRepository>();

            var notifications = await notificationRepository.GetNotificationsByUserIdAsync(userId);

            if (notifications == null || !notifications.Any())
            {
                _logger.LogWarning($"No notifications found for user: {userId}");
                throw new ArgumentNullException($"No notifications found for user with ID {userId}.");
            }

            return notifications;
        }

        public async ValueTask DisposeAsync()
        {
            if (_processor != null)
            {
                await _processor.StopProcessingAsync();
                await _processor.DisposeAsync();
            }

            if (_client != null)
            {
                await _client.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}