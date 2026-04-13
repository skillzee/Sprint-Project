# TravelApp System Architecture

This document provides a detailed overview of the system architecture, communication patterns, and design principles used in TravelApp.

## 🏗️ High-Level Architecture

TravelApp is built using a **Microservices Architecture**. The system is decomposed into small, independent services that communicate over the network. This approach allows for independent scaling, technology flexibility, and improved fault tolerance.


---

## 📡 Communication Patterns

### Synchronous Communication (REST/HTTP)
Most client-to-service and some service-to-service calls are synchronous.
- **Client to Gateway**: HTTPS requests.
- **Gateway to Services**: Internal HTTP requests routed via Ocelot.
- **Inter-service**: Direct HTTP calls for immediate data requirements (e.g., Booking service verifying hotel availability).

### Asynchronous Communication (Messaging)
For non-blocking operations and eventual consistency, the system uses **RabbitMQ**.
- **Event-Driven Notifications**: When a booking is confirmed or a hotel status changes, an event is published to RabbitMQ.
- **Decoupling**: The publishing service doesn't need to know about the subscribers (e.g., Notification Service), ensuring high decoupling.

---

## 💾 Data Management

### Database per Service
Each service (where applicable) has its own database schema to ensure data isolation.
- **SQL Server**: Used for relational data requiring ACID properties (Users, Bookings, Hotel Listings).
- **Redis Cache**: Used for high-speed access to frequently requested or transient data (Flight availability, Hotel lookups).

---

## 🔐 Security & Identity

- **Authentication**: Centralized in the **Auth Service**. Upon login, it issues a **JWT (JSON Web Token)**.
- **Authorization**: The API Gateway and individual services validate the JWT. Role-based access control (RBAC) is implemented to restrict access to admin/user specific endpoints.
- **Gateway Security**: Acts as the single entry point, hiding the internal network structure of the microservices.

---

## 🧩 Shared Components

The `TravelApp.Shared` library contains cross-cutting concerns used by multiple services:
- **Global Error Handling**: Middleware to ensure consistent error responses across all APIs.
- **Common DTOs & Events**: Shared data contracts for inter-service communication.
- **Logging & Helpers**: Utility classes for standardizing common tasks.

---

## 🚀 Deployment Architecture

- **Containerization**: Every service is Dockerized with its own `Dockerfile`.
- **Orchestration**: `docker-compose.yml` manages the lifecycle of the entire system, including infrastructure components like SQL Server, Redis, and RabbitMQ.
- **Scalability**: Services are designed to be stateless, allowing them to be scaled horizontally behind a load balancer or within a Kubernetes cluster.
