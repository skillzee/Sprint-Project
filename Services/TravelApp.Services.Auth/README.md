# TravelApp - Auth Service

The **Auth Service** is responsible for identity management, authentication, and authorization across the TravelApp ecosystem.

## 🚀 Purpose

- **User Management**: Registration and profile management.
- **Authentication**: Secure login using JWT (JSON Web Tokens).
- **Authorization**: Defining user roles and permissions.
- **Security**: Password hashing and token generation.

---

## 🏗️ Architecture

This service follows a **Layered Architecture**:
- **Controllers**: Handle HTTP requests and define API endpoints.
- **Services**: Contain business logic for authentication and user management.
- **Repositories**: Handle data access logic using Entity Framework Core.
- **Models/DTOs**: Define data structures and data transfer objects.
- **Data (DbContext)**: Entity Framework context for SQL Server.

---

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT, ASP.NET Core Identity (if applicable)

---

## 🔐 Key Features

- **JWT Issuance**: Generates a secure token upon successful login.
- **Role-Based Access Control**: Supports different roles (User, Agent, Admin).
- **Environment Driven**: Sensitive configurations (JWT Key, Connection Strings) are managed via `.env` or `appsettings.json`.

---

## 🚀 Running the Service

### Prerequisites
- .NET 10.0 SDK
- SQL Server instance

### Setup
1. Configure your connection string and JWT settings in `.env`.
2. Apply migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the service:
   ```bash
   dotnet run
   ```

Default port: `5001`.
