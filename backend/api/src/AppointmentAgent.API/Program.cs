using System.Text.Json.Serialization;
using AppointmentAgent.API.Endpoints;
using AppointmentAgent.Domain;
using AppointmentAgent.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

// Service Registration
services.AddPersistence(configuration.GetConnectionString("DefaultConnection"));
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
services.AddDomainServices();
services.AddCors(options =>
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(configuration["FRONTEND_ORIGIN_URL"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    })
);

// Build Application
var app = builder.Build();

// Cors Policies
app.UseCors("FrontendPolicy");

// Database Setup
var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

try
{
    await dbContext.Database.MigrateAsync();
}
catch (Exception exception)
{
    var logger = app.Logger;
    logger.LogError(exception, "Database setup failed.");
    throw;
}

// API Setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// API Endpoints Registration
AppointmentEndpoints.Map(app);

app.Run();