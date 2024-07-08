
# Async Request-Reply Example API with Azure Service Bus

## Overview
This project demonstrates an asynchronous request-reply pattern using .NET 8 Web API and Azure Service Bus topics. It includes endpoints to submit a request, check its status, and handle asynchronous processing.

## Prerequisites
- .NET 8 SDK
- Azure Subscription
- Azure Service Bus Namespace

## Setup

### 1. Clone the Repository
```bash
git clone git@github.com:harishnarain/AsyncRequestReply.git
```

### 2. Configuration
Add the Azure Service Bus configuration details to `appsettings.json`.

#### appsettings.json
```json
{
  "AzureServiceBus": {
    "ConnectionString": "your_service_bus_connection_string",
    "RequestTopicName": "request-topic",
    "ResponseTopicName": "response-topic",
    "SubscriptionName": "response-subscription"
  }
}
```

### 3. Create the Service Bus Namespace
This step only needs to be run once. If you have already ran this when following the README for the Worker microservice then disregard this step now.

Create the Service Bus namespace and create the topics and subscriptions based on what you want. A Bicep file is included in this project to help with creating the Service Bus namespace.

### 4. Build and Run
```bash
cd FrontendApi
dotnet build
dotnet run
```

The application will start and listen for incoming REST API calls.

The service will run on port: TCP/5004

## API Endpoints

1. **POST /request**
   - **Description**: Submit a new request.
   - **Request Body**:
     ```json
     {
       "Content": "Sample request message"
     }
     ```
   - **Response**:
     - **202 Accepted** with `Location` header pointing to `/status/{generated-id}`.

2. **GET /status/{id}**
   - **Description**: Check the status of a request.
   - **Response**:
     - **202 Accepted** if the request is still being processed.
     - **201 Created** with a JSON body containing the request details and the redirect URL if the request is completed.
       ```json
       {
         "id": "{generated-id}",
         "redirectUrl": "http://localhost:5160/api/messages/{generated-id}"
       }
       ```

## Project Structure

```
FrontendApi/
│
├── Controllers/
│   ├── RequestController.cs
|   └── StatusController.cs
│
├── Models/
│   ├── RequestData.cs
|   └── RequestStatus.cs 
│
├── Services/
│   └── ServiceBusClientService.cs
│
├── Program.cs
├── appsettings.json
└── FrontendApi.csproj
```

## How It Works

### REST API with Asynchronous Request and Reply

1. The caller sends a POST request with a JSON message with a body of `{ "Content": "enter some message here" }`. *See API Endpoints*
2. If the message is accepted a **HTTP 202** will be received and provide a redirect URL to check the status of the request.
3. A long poll can then be done on the redirect URL which is the status endpoint. *See API Endpoints*
4. The message is then placed in the `request-topic` in Service Bus.
5. The Worker service is a subscriber of the `request-topic` and thus picks up the message and processes it.
6. The Worker service then puts a response message into the `response-topic` in Service Bus.
7. The Front End API's background service is a subscriber of the `response-topic`. Once the response message is received, the message will be processed and the status will be updated for the ID. The response body of the status request will return an **HTTP 201** and provide a redirect URI. This URI is on the worker endpoint and is just used for querying the records that get created in the in memory data store with each request.

### Asynchronous Request Reply

The above exercise demonstrates how by using the Asynchronous Request Reply pattern, long running tasks such as the *Worker Microservice* in this can perform it's tasks meanwhile the Frontend API can provide the caller to check on the status of the request without staying connected. The Front End API can update it's status once a response is provided by the *Worker Microservice*

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests.

## License

This project is licensed under the MIT License.
