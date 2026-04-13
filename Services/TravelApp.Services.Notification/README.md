# TravelApp - Notification Service

The **Notification Service** is an event-driven component responsible for delivering real-time alerts and messages to users.

## 🚀 Purpose

- **Real-time Alerts**: Notify users about booking confirmations, cancellations, and hotel status updates.
- **Asynchronous Processing**: Consumes events from RabbitMQ to ensure notifications don't block the main business workflows.
- **Multi-channel (Ready)**: Designed to support various notification channels (Email, SMS, Push).

---

## 🏗️ Architecture

- **Event Consumers**: Dedicated consumer classes (e.g., `BookingConfirmedConsumer`) that listen to specific RabbitMQ queues.
- **Stateless Workers**: Designed as a background worker service or a Web API with background consumers.
- **Integration**: Heavily dependent on the messaging infrastructure (RabbitMQ).

---

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Messaging**: RabbitMQ
- **Communication**: MassTransit (or direct RabbitMQ Client)

---

## 📥 Subscribed Events

This service listens for the following events:
- `BookingConfirmedEvent`
- `BookingCancelledEvent`
- `HotelApprovedEvent`
- `HotelRejectedEvent`

---

## 🚀 Running the Service

### Prerequisites
- .NET 10.0 SDK
- RabbitMQ Server

### Commands
```bash
dotnet run
```

Default port: `5093` (Downstream mapping in Gateway).
