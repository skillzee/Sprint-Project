# TravelApp - API Gateway

The **Gateway** service is the central entry point for all client requests in the TravelApp ecosystem. It is built using **Ocelot**, a lightweight API Gateway for .NET.

## 🚀 Purpose

In a microservices architecture, having a single entry point simplifies client communication by:
- **Routing**: Directing requests to the appropriate downstream microservice.
- **Aggregation**: Combining results from multiple services (if configured).
- **Security**: Validating JWT tokens before requests reach downstream services.
- **Abstraction**: Hiding the complexity and internal network structure of the microservices.

---

## 🏗️ Architecture

- **Framework**: ASP.NET Core 10.0
- **Gateway Engine**: Ocelot
- **Logic**: Configuration-driven routing (defined in `ocelot.json`).

---

## ⚙️ Configuration

The routing logic is defined in `ocelot.json`. Each entry defines an upstream path (exposed to the client) and a downstream path (internal service endpoint).

### Key Routes
- `/api/auth/*` -> Auth Service
- `/api/booking/*` -> Booking Service
- `/api/flight/*` -> Flight Service
- `/api/hotel/*` -> Hotel Service
- `/api/trip/*` -> Trip Service
- `/api/notification/*` -> Notification Service

---

## 🚀 Running the Gateway

### Prerequisites
- .NET 10.0 SDK

### Standard Run
```bash
dotnet run
```

The gateway typically runs on `http://localhost:5000` (mapped to `8080` in Docker).

---

## 🔒 Security

The Gateway is configured to expect a Bearer Token (JWT) for protected routes. It validates the token and extracts claims that are then passed to the downstream services.
