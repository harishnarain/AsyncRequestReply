using Azure.Identity;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Worker.Models;
using Worker.Repositories;

namespace Worker.Services
{
    public class ServiceBusService : BackgroundService, IServiceBusService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly ServiceBusSender _sender;
        private readonly IRecordRepository _repository;
        private readonly string _requestTopicName;
        private readonly string _responseTopicName;
        private readonly string _subscriptionName;

        public ServiceBusService(IRecordRepository repository, IConfiguration configuration)
        {
            _repository = repository;

            var credential = new DefaultAzureCredential();
            var clientNamespace = configuration["AzureServiceBus:Namespace"];
            _requestTopicName = configuration["AzureServiceBus:RequestTopicName"];
            _responseTopicName = configuration["AzureServiceBus:ResponseTopicName"];
            _subscriptionName = configuration["AzureServiceBus:SubscriptionName"];

            _client = new ServiceBusClient(clientNamespace, credential);
            _processor = _client.CreateProcessor(_requestTopicName, _subscriptionName, new ServiceBusProcessorOptions());
            _sender = _client.CreateSender(_responseTopicName);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var messageBody = args.Message.Body.ToString();
            var requestMessage = JsonSerializer.Deserialize<Record>(messageBody);

            if (requestMessage == null)
            {
                await args.DeadLetterMessageAsync(args.Message, "Invalid message format", "The message body could not be deserialized.");
                return;
            }

            var record = new Record
            {
                Id = requestMessage.Id,
                Content = requestMessage.Content,
                Status = "Processed"
            };
            _repository.Add(record);

            var responseMessage = new ServiceBusMessage(JsonSerializer.Serialize(record))
            {
                CorrelationId = requestMessage.Id.ToString() // Set the correlation ID to match the request ID
            };
            await _sender.SendMessageAsync(responseMessage);

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Message handler encountered an exception {args.Exception}.");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}
