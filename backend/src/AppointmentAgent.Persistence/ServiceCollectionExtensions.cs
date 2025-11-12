using AppointmentAgent.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentAgent.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, 
        string? connectionString
    )
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        
        // Repositories
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        
        return services;
    }
}