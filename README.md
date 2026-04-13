# TravelApp - Microservices Travel Management System

Welcome to **TravelApp**, a modern, scalable, and high-performance travel management system built using a microservices architecture. This project enables users to search for flights, book hotels, manage trips, and receive real-time notifications.

## 🚀 Overview

TravelApp is designed to handle complex travel workflows by decomposing the system into independently deployable services. Each service is built with .NET (C#) and follows clean architecture principles.

### Key Features
- **Centralized Authentication**: JWT-based secure auth via Auth Service.
- **Flight Management**: Search and manage flight details with Redis caching.
- **Hotel & Booking**: Comprehensive hotel listings and seamless booking workflows.
- **Intelligent Trip Planning**: Trip management with AI-powered suggestions (Gemini API integration).
- **Real-time Notifications**: Event-driven notifications via RabbitMQ.
- **Unified Gateway**: API Gateway using Ocelot for streamlined request routing.
- **Responsive Web UI**: Modern Angular-based frontend.

---

## 🏗️ Architecture

The system follows a microservices architecture style with both synchronous and asynchronous communication patterns.

> [!TIP]
> For a deep dive into the system design, communication patterns, and infrastructure details, please refer to the **[ARCHITECTURE.md](./ARCHITECTURE.md)** file.

---

## 🛠️ Tech Stack

### Backend
- **Framework**: .NET 10.0 / ASP.NET Core
- **API Gateway**: Ocelot
- **Database**: SQL Server (Entity Framework Core)
- **Caching**: Redis
- **Messaging**: RabbitMQ
- **Authentication**: JWT (JSON Web Tokens)

### Frontend
- **Framework**: Angular
- **Styling**: CSS / Bootstrap

### DevOps & Infrastructure
- **Containerization**: Docker & Docker Compose
- **Continuous Integration**: GitHub Actions (defined in `.github/workflows`)

---

## 📂 Project Structure

```bash
TravelApp/
├── Gateway/             # API Gateway (Ocelot)
├── Services/            # Microservices
│   ├── Auth/            # Authentication & Authorization
│   ├── Booking/         # Room Bookings & Logic
│   ├── Flight/          # Flight Information
│   ├── Hotel/           # Hotel & Room Management
│   ├── Notification/    # Event-driven Notifications
│   └── Trip/            # Trip Planning (Gemini AI integration)
├── Shared/              # Common libraries & middleware
├── Web/                 # Angular Frontend
├── Tests/               # Unit & Integration Tests
└── docker-compose.yml   # Infrastructure Orchestration
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js & npm](https://nodejs.org/) (for Angular Web UI)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (if running locally without Docker)

### Running with Docker (Recommended)
1. Clone the repository.
2. Ensure Docker is running.
3. Run the following command in the root directory:
   ```bash
   docker-compose up -d
   ```
4. Access the application:
   - **Frontend**: http://localhost:80
   - **API Gateway**: http://localhost:5000
   - **RabbitMQ Management**: http://localhost:15672

### Running Locally (Development Mode)
Each service can be run individually using `dotnet run`, and the frontend using `npm start`. Ensure that the infrastructure (SQL Server, Redis, RabbitMQ) is available and connection strings are correctly configured in `.env` or `appsettings.json`.

---

## 📜 Documentation

- **Architecture Details**: [ARCHITECTURE.md](./ARCHITECTURE.md)
- **Service Specific READMEs**:
    - [Auth Service](./Services/TravelApp.Services.Auth/README.md)
    - [Booking Service](./Services/TravelApp.Services.Booking/README.md)
    - [Flight Service](./Services/TravelApp.Services.Flight/README.md)
    - [Hotel Service](./Services/TravelApp.Services.Hotel/README.md)
    - [Notification Service](./Services/TravelApp.Services.Notification/README.md)
    - [Trip Service](./Services/TravelApp.Services.Trip/README.md)
    - [Gateway](./Gateway/TravelApp.Gateway/README.md)
