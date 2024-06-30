
# Asynchronous Request Reply with Azure Service Bus

This repository is an example project demonstrating how to use the Asynchronous Request Reply pattern with Azure Service Bus. The solution consists of a Frontend API and a Backend Worker Microservice.

## Project Structure

- `FrontendApi/`: Contains the Frontend API project.
- `Worker/`: Contains the Backend Worker Microservice project.
- `Bicep/`: Contains the Bicep templates for creating the Service Bus resource in Azure.

## Overview

### Frontend API

The Frontend API receives a call via its REST endpoint. The request body is a JSON document in the following format:
```json
{ "Content": "some message here" }
```
Upon receiving the document, the API places the message into the request service bus topic. The caller then receives an HTTP 202 response with a redirect URL to the status endpoint. The caller can perform long polling on this status endpoint. While the long-running process is still executing, the status endpoint will return HTTP 202. Once the process is complete, it returns HTTP 201 with a redirect URL in the status body.

### Backend Worker Microservice

The Backend Worker Microservice subscribes to the request topic and processes the messages. It deserializes the message, creates a record in its in-memory datastore, and sends a response message to the response service bus topic. The Frontend API has a background process that subscribes to this response topic, updates the status accordingly, and informs the caller via the status endpoint.

## Building and Running

### Prerequisites

- .NET 6 SDK
- Azure Subscription
- Azure Service Bus

### Steps

1. Clone the repository:
   ```sh
   git clone git@github.com:harishnarain/AsyncRequestReply.git
   ```

2. Navigate to the `Bicep/` folder and deploy the Service Bus resource:
   ```sh
   az deployment group create --resource-group <your-resource-group> --template-file servicebus.bicep
   ```

3. Build and run the Frontend API:
   ```sh
   cd FrontendApi
   dotnet build
   dotnet run
   ```

4. Build and run the Backend Worker Microservice:
   ```sh
   cd Worker
   dotnet build
   dotnet run
   ```

Follow the respective README files in the `FrontendApi` and `Worker` folders for more details.

## Conclusion

This project showcases how to implement the Asynchronous Request Reply pattern using Azure Service Bus in a .NET Core solution. By following the steps provided, you can understand and replicate this pattern in your own projects.

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests.

## License

This project is licensed under the MIT License.
