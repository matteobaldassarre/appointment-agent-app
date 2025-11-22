using AppointmentAgent.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentAgent.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentService, AppointmentService>();
        
        return services;
    }
}