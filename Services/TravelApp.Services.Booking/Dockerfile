# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy both the Service API and Shared project files
COPY ["Services/TravelApp.Services.Booking/TravelApp.Services.Booking.csproj", "Services/TravelApp.Services.Booking/"]
COPY ["Shared/TravelApp.Shared/TravelApp.Shared.csproj", "Shared/TravelApp.Shared/"]
RUN dotnet restore "Services/TravelApp.Services.Booking/TravelApp.Services.Booking.csproj"

# Copy everything else
COPY . .
WORKDIR "/src/Services/TravelApp.Services.Booking"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TravelApp.Services.Booking.dll"]
