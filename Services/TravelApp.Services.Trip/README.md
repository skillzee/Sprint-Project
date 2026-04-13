# TravelApp - Trip Service

The **Trip Service** provides comprehensive trip management and intelligent planning features, enhanced by AI integration.

## 🚀 Purpose

- **Trip Management**: Allows users to create, organize, and track their travel itineraries.
- **AI Planning**: Integrates with the **Gemini AI API** to suggest itineraries and travel tips.
- **Integration**: Combines data from flight and hotel segments into a unified trip view.

---

## 🏗️ Architecture

- **Layered Architecture**: Standard Controllers/Services/Repositories structure.
- **External Integration**: Dedicated service layer for communicating with the Gemini AI model.
- **Caching**: Uses Redis to store generated itineraries and reduce redundant AI calls.

---

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server
- **Caching**: Redis
- **AI Integration**: Gemini API

---

## 🤖 AI Features

The Trip Service uses Google's Gemini API to:
- Generate personalized travel itineraries.
- Provide destination recommendations.
- Offer travel tips based on the user's trip preferences.

---

## 🚀 Running the Service

### Prerequisites
- .NET 10.0 SDK
- SQL Server
- Redis
- **GEMINI_API_KEY**: A valid API key for Google Gemini.

### Setup
1. Add your `GEMINI_API_KEY` to the `.env` file.
2. Apply migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the service:
   ```bash
   dotnet run
   ```

Default port: `5179`.
