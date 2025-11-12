using AppointmentAgent.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var configuration = builder.Configuration;
var services = builder.Services;

services.AddPersistence(configuration.GetConnectionString("DefaultConnection"));

app.Run();