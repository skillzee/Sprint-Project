# TravelApp - Web Client

The **TravelApp Web Client** is a modern, responsive frontend application built with **Angular**.

## 🚀 Purpose

- **User Interface**: Provides an intuitive interface for searching flights, booking hotels, and managing trips.
- **Role-based Experience**: Tailored experiences for regular Users and Hotel Agents.
- **Client-side Logic**: Handles form validations, state management, and interaction with the API Gateway.

---

## 🏗️ Architecture

- **Framework**: Angular
- **State Management**: Service-based state management or RxJS-driven streams.
- **Routing**: Angular Router for seamless single-page application navigation.
- **Styling**: CSS / Bootstrap for a clean and responsive design.

---

## 🛠️ Tech Stack

- **Framework**: Angular
- **Language**: TypeScript
- **Styling**: CSS
- **Inter-service**: Communicates with the Ocelot API Gateway.

---

## 🚀 Running the Web App

### Prerequisites
- Node.js (v18+)
- npm

### Setup & Run
1. Navigate to the web directory:
   ```bash
   cd Web/travel-app-web
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Run the development server:
   ```bash
   npm start
   ```

The application will be available at `http://localhost:4200` (or as configured). In production/Docker environments, it is typically served on port `80`.

---

## 🔐 Authentication

The web app stores the JWT received from the Auth Service (via the Gateway) in local storage or session storage and includes it in the `Authorization` header for all protected API calls.
