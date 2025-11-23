# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/AppointmentAgent.API/*.csproj ./AppointmentAgent.API/
COPY src/AppointmentAgent.Domain/*.csproj ./AppointmentAgent.Domain/
COPY src/AppointmentAgent.Persistence/*.csproj ./AppointmentAgent.Persistence/

COPY src/ .

RUN dotnet restore "./AppointmentAgent.API/AppointmentAgent.API.csproj"

WORKDIR /src/AppointmentAgent.API
RUN dotnet publish "AppointmentAgent.API.csproj" -c Release -o /app/publish --no-restore

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AppointmentAgent.API.dll"]
