# Using OpenTelemetry

This is a demo project on how to use OpenTelemetry in dotnet. This is trying to simulate a microservice platform, so we have 3 services with some dependency between them using direct http requests or messages through queues.

## Services

### Customer.Api
This is the entry point, provides an API to manage customers (simple CRUD API). It stores the data in a database and after a customer is added, updated or deleted, emits a message for its consumers.

### FraudCheck.Api
A dependency of `Customer.Api`, it has an endpoint that randomly returns `OK 200` or `Bad Request 400` when creating a customer. When a customer is flagged as possible fraud, a message is published.

### EmailSender
Consumes messages about customer changes, simulating an email service sending comms to the customer about these changes.

## Tech details
The solution uses [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)[^1] to create the infra needed:
- Postgres database for `Customer.Api` to store its data
- RabbitMQ to send and receive events
- OpenTelemetry infra: Jaeger for traces, Prometheus for metrics and Loki for logs, all using the OTEL collector
- Grafana with predefined dashboards to visualise the telemetry

[^1]: The infra runs in Docker containers, but because my setup is using Docker on WSL without Docker Desktop, paths to files are [absolute](src/AppHost/Modules/Grafana.cs) and the Aspire dashboard is empty because the OTEL collector container can't push the data into the Windows host.