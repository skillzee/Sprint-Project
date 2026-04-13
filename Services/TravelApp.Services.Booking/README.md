# TravelApp - Booking Service

The **Booking Service** handles the core logic for room reservations and hotel bookings within the TravelApp ecosystem.

## 🚀 Purpose

- **Reservation Management**: Creation, updating, and cancellation of hotel room bookings.
- **Availability Check**: Communicates with the Hotel Service to verify room availability before booking.
- **Asynchronous Events**: Publishes events to RabbitMQ when bookings are confirmed or cancelled to trigger notifications.

---

## 🏗️ Architecture

This service follows a **Layered Architecture** with integration for **Event-Driven Communication**:
- **Controllers**: API endpoints for booking operations.
- **Services**: Business logic including date overlap validation and availability checks.
- **Repositories**: Data access layer for booking records.
- **Messaging**: Integration with RabbitMQ to publish booking events.

---

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Messaging**: RabbitMQ
- **Service-to-Service**: HttpClient for synchronous calls to Hotel Service.

---

## 🔄 Event Workflows

1. **Booking Confirmed**: When a user successfully books a room, the service publishes a `BookingConfirmedEvent`.
2. **Booking Cancelled**: If a booking is revoked, a `BookingCancelledEvent` is published.
3. **Notification**: The Notification Service consumes these events to alert the user.

---

## 🚀 Running the Service

### Prerequisites
- .NET 10.0 SDK
- SQL Server
- RabbitMQ

### Commands
```bash
dotnet ef database update
dotnet run
```

Default port: `5112`.
