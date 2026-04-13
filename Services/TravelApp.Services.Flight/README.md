# TravelApp - Flight Service

The **Flight Service** provides functionality for searching and managing flight information within the TravelApp ecosystem.

## 🚀 Purpose

- **Flight Search**: allows users to find flights based on various criteria.
- **Flight Management**: Administrative interface for managing flight schedules and details.
- **High Performance**: Utilizes Redis caching to ensure fast response times for flight lookups.

---

## 🏗️ Architecture

- **Clean Architecture Principles**: Organized into logical layers for maintainability.
- **Caching Layer**: Integrated Redis storage to reduce database load and improve latency for frequent searches.
- **Data Access**: Entity Framework Core for primary data storage (if applicable) and Redis for transient/cached data.

---

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server/Redis
- **Caching**: Redis
- **ORM**: Entity Framework Core

---

## ⚡ Performance Optimization

- **Redis Integration**: Flight schedules and availability are cached in Redis.
- **Stateless Design**: Allows for horizontal scaling to handle high traffic during peak travel seasons.

---

## 🚀 Running the Service

### Prerequisites
- .NET 10.0 SDK
- Redis Server
- SQL Server

### Commands
```bash
dotnet run
```

Default port: `5007`.
