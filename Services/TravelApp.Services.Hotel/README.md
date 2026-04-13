# TravelApp - Hotel Service

The **Hotel Service** manages hotel listings, room types, and room availability in the TravelApp ecosystem.

## 🚀 Purpose

- **Hotel Management**: CRUD operations for hotels and their properties.
- **Room Management**: Details about room types, pricing, and amenities.
- **Availability Tracking**: Maintaining the inventory of available rooms for the Booking Service.
- **Agent Portal Support**: Specific logic for hotel agents to list and manage their properties.

---

## 🏗️ Architecture

- **Service Layer**: Handles complex business logic including hotel approval workflows.
- **Repository Pattern**: Abstracts data access using Entity Framework Core.
- **Caching**: Uses Redis to cache hotel search results for optimal performance.
- **Messaging**: Publishes `HotelApproved` or `HotelRejected` events to RabbitMQ.

---

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server
- **Caching**: Redis
- **Messaging**: RabbitMQ
- **ORM**: Entity Framework Core

---

## 🏢 Business Logic: Approval Workflow

Hotels registered by Agents undergo an approval process:
1. **Registered**: Initial state.
2. **Review**: Admin reviews the hotel details.
3. **Approved/Rejected**: An event is published via RabbitMQ to notify the agent.

---

## 🚀 Running the Service

### Prerequisites
- .NET 10.0 SDK
- SQL Server
- Redis
- RabbitMQ

### Commands
```bash
dotnet ef database update
dotnet run
```

Default port: `5165`.
