
# Worker Microservice

This project is a .NET 8 microservice that processes messages from an Azure Service Bus Topic. It performs the following tasks:
- Receives messages from an Azure Service Bus Topic as a subscriber.
- Adds a record based on the message received to an in-memory datastore.
- Sends an acknowledgment message to another Azure Service Bus Topic after processing the message.
- Provides a REST API endpoint to retrieve a list of all documents from the in-memory datastore for diagnostics.

## Features

- Asynchronous message processing using Azure Service Bus.
- In-memory data storage for records.
- REST API for retrieving records.
- Correlation ID support for tracking messages.

## Prerequisites

- .NET 8 SDK
- Azure Service Bus Namespace and Topics

## Setup

### 1. Clone the Repository

```bash
git clone git@github.com:harishnarain/AsyncRequestReply.git
```

### 2. Configuration

Add your Azure Service Bus connection string and topics configuration to the `appsettings.json` file.

```json
{
    "ServiceBus": {
        "ConnectionString": "YourServiceBusConnectionString",
        "RequestTopic": "request-topic",
        "ResponseTopic": "response-topic",
        "SubscriptionName": "request-subscription"
    }
}
```

### 3. Create the Service Bus Namespace
This step only needs to be run once. If you have already ran this when following the README for the FrontendApi then disregard this step now.

Create the Service Bus namespace and create the topics and subscriptions based on what you want. An included Bicep file is included in this project to help with creating the Service Bus namespace.

### 4. Build and Run
```bash
cd Worker
dotnet build
dotnet run
```

The application will start and listen for incoming messages on the specified Azure Service Bus topics.

There is a REST API endpoint used for viewing the objects in the in memory datastore. It's accessible on port: TCP/5160

## API Endpoints

### Retrieve All Records

- **URL**: `/api/messages`
- **Method**: `GET`
- **Description**: Retrieves a list of all records from the in-memory datastore.

### Retrieve a Specific Record

- **URL**: `/api/messages/{id}`
- **Method**: `GET`
- **Description**: Retrieves a specific record by ID from the in-memory datastore.

## Project Structure

```
WorkerMicroservice/
│
├── Controllers/
│   └── MessagesController.cs
│
├── Models/
│   └── Record.cs
│
├── Repositories/
│   ├── IRecordRepository.cs
│   └── InMemoryRecordRepository.cs
│
├── Services/
│   ├── IServiceBusService.cs
│   └── ServiceBusService.cs
│
├── Program.cs
├── appsettings.json
└── WorkerMicroservice.csproj
```

## How It Works

### Message Processing

1. The service listens for incoming messages on the `request-topic`.
2. When a message is received, it is deserialized to a `Record` object.
3. The record is added to an in-memory datastore.
4. An acknowledgment message is sent to the `response-topic` with the correlation ID matching the received message ID.
5. The message is completed in the Azure Service Bus queue.

### REST API

The REST API provides endpoints to retrieve records from the in-memory datastore. It supports fetching all records or a specific record by its ID.

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests.

## License

This project is licensed under the MIT License.
