using AppointmentAgent.Persistence;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var serviceCollection = builder.Services;
var configuration = app.Configuration;

var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

// Register services in DI Container
serviceCollection.AddPersistence(connectionString);

app.Run();