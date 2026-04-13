# TravelApp - Shared Library

The **TravelApp.Shared** library contains common code, utilities, middleware, and data contracts used across all microservices.

## 🚀 Purpose

- **Code Reuse**: Avoid duplicating common logic in every service.
- **Consistency**: Ensure uniform error handling, logging, and security practices.
- **Contract Management**: Centralized definition of DTOs and Event models used for inter-service communication.

---

## 🏗️ Components

### Middleware
- **GlobalExceptionHandler**: Standardizes API error responses (Problem Details) across the entire system.
- **LoggingMiddleware**: Ensures consistent request/response logging.

### Utilities
- **Helper Classes**: Common string manipulations, date formatting, and validation logic.

### Contracts
- **DTOs**: Shared data transfer objects for API responses.
- **Events**: Shared models for RabbitMQ messages (e.g., `BookingConfirmedEvent`).

---

## 🛠️ Tech Stack

- **Framework**: .NET 10.0 Class Library

---

## 📦 Usage

To use the shared library in a microservice:
1. Add a project reference to `TravelApp.Shared.csproj`.
2. Register the shared services or middleware in `Program.cs`.

Example registration of Global Exception Handler:
```csharp
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
```
