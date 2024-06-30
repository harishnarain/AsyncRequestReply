using Azure.Identity;
using Azure.Messaging.ServiceBus;
using FrontendApi.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace FrontendApi.Services
{
    public class ServiceBusClientService
    {
        private readonly ServiceBusClient _client;
        private readonly string _requestTopicName;
        private readonly string _responseTopicName;
        private readonly string _subscriptionName;
        private readonly ServiceBusProcessor _processor;
        private readonly ConcurrentDictionary<string, RequestStatus> _requestStatuses;

        public ServiceBusClientService(IConfiguration configuration)
        {
            _requestTopicName = configuration["AzureServiceBus:RequestTopicName"];
            _responseTopicName = configuration["AzureServiceBus:ResponseTopicName"];
            _subscriptionName = configuration["AzureServiceBus:SubscriptionName"];

            var credential = new DefaultAzureCredential();
            var clientNamespace = configuration["AzureServiceBus:Namespace"];
            _client = new ServiceBusClient(clientNamespace, credential);
            _processor = _client.CreateProcessor(_responseTopicName, _subscriptionName);
            _requestStatuses = new ConcurrentDictionary<string, RequestStatus>();
        }

        public async Task SendMessageAsync(RequestData requestData)
        {
            var sender = _client.CreateSender(_requestTopicName);
            var messageBody = JsonSerializer.Serialize(requestData);
            var message = new ServiceBusMessage(messageBody)
            {
                CorrelationId = requestData.Id
            };

            _requestStatuses[requestData.Id] = new RequestStatus
            {
                Id = requestData.Id,
                IsCompleted = false,
                RedirectUrl = null
            };

            await sender.SendMessageAsync(message);
        }

        public RequestStatus GetRequestStatus(string id)
        {
            if (_requestStatuses.TryGetValue(id, out var status))
            {
                return status;
            }

            return null;
        }

        public async Task StartProcessingResponses()
        {
            async Task ProcessMessage(ProcessMessageEventArgs args)
            {
                var correlationId = args.Message.CorrelationId;

                if (_requestStatuses.TryGetValue(correlationId, out var status))
                {
                    status.IsCompleted = true;
                    status.RedirectUrl = $"http://localhost:5160/api/messages/{correlationId}";
                }

                await args.CompleteMessageAsync(args.Message);
            }

            Task ProcessError(ProcessErrorEventArgs args)
            {
                // Handle errors here
                return Task.CompletedTask;
            }

            _processor.ProcessMessageAsync += ProcessMessage;
            _processor.ProcessErrorAsync += ProcessError;

            await _processor.StartProcessingAsync();
        }
    }
}
