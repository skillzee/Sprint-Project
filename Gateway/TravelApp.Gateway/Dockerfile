# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Gateway doesn't reference Shared directly but we'll include it just in case, or just restore Gateway
COPY ["Gateway/TravelApp.Gateway/TravelApp.Gateway.csproj", "Gateway/TravelApp.Gateway/"]
RUN dotnet restore "Gateway/TravelApp.Gateway/TravelApp.Gateway.csproj"

# Copy everything else
COPY . .
WORKDIR "/src/Gateway/TravelApp.Gateway"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TravelApp.Gateway.dll"]
