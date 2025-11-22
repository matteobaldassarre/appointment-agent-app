using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AppointmentAgent.API.Endpoints;

public static class AppointmentEndpoints
{
    public static void Map(WebApplication app)
    {
        var appointments = app
            .MapGroup("/appointments")
            .WithTags("Appointments");
        
        // Get a list of all scheduled appointments
        appointments.MapGet("", GetAllAppointments);
        
        // Get a specific appointment
        appointments.MapGet("/{id:guid}", GetAppointment);
        
        // Create a new appointment
        appointments.MapPost("", CreateAppointment);
        
        // Update an existing appointment
        appointments.MapPut("/{id:guid}", UpdateAppointment);
        
        // Delete an existing appointment
        appointments.MapDelete("/{id:guid}", DeleteAppointment);
    }
    
    public static async Task<Ok<IEnumerable<Appointment>>> GetAllAppointments(
        IAppointmentService appointmentService, 
        CancellationToken cancellationToken
    )
    {
        var appointments = await appointmentService.GetAllAsync(cancellationToken);
        
        return TypedResults.Ok(appointments);
    }
    
    public static async Task<Results<Ok<Appointment>, NotFound>> GetAppointment(
        Guid id, 
        IAppointmentService appointmentService, 
        CancellationToken cancellationToken
    )
    {
        var existingAppointment = await appointmentService.GetByIdAsync(id, cancellationToken);
                
        if (existingAppointment is null) 
            return TypedResults.NotFound();

        return TypedResults.Ok(existingAppointment);
    }
    
    public static async Task<Results<Created<Appointment>, UnauthorizedHttpResult>> CreateAppointment(
        HttpRequest request, 
        Appointment? newAppointment, 
        IAppointmentService appointmentService,
        IConfiguration configuration,
        CancellationToken cancellationToken
    )
    {
        await appointmentService.CreateAsync(newAppointment, cancellationToken);
        
        return TypedResults.Created($"/appointments/{newAppointment.Id}", newAppointment);
    }
    
    public static async Task<Results<NoContent, NotFound>> UpdateAppointment(
        Guid id,
        Appointment updatingAppointment, 
        IAppointmentService appointmentService, 
        CancellationToken cancellationToken
    )
    {
        var updateResult = await appointmentService.TryUpdateAsync(id, updatingAppointment, cancellationToken);
                
        if (updateResult is false) 
            return TypedResults.NotFound();
        
        return TypedResults.NoContent();
    }
    
    public static async Task<Results<NoContent, NotFound>> DeleteAppointment(
        Guid id, 
        IAppointmentService appointmentService, 
        CancellationToken cancellationToken
    )
    {
        var deleteResult = await appointmentService.TryDeleteAsync(id, cancellationToken);
                
        if (deleteResult is false) 
            return TypedResults.NotFound();
                
        return TypedResults.NoContent();
    }
}