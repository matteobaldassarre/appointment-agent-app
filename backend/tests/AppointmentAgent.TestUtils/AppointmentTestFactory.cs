using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Entities.Enums;

namespace AppointmentAgent.TestUtils;

public static class AppointmentTestFactory
{
    public static Appointment CreateAppointment(Action<Customer>? customerConfiguration = null)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Matteo",
            LastName = "Baldassarre",
            Phone = "3333333333"
        };
        
        customerConfiguration?.Invoke(customer);

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            Customer = customer,
            Date = DateTime.UtcNow.AddDays(2),
            Status = AppointmentStatus.Scheduled
        };

        customer.Appointments = [appointment];

        return appointment;
    }
    
    public static Appointment CreateAppointment(
        Guid customerId, 
        Guid appointmentId,
        Action<Customer>? customerConfiguration = null
    )
    {
        var customer = new Customer
        {
            Id = customerId,
            FirstName = "Matteo",
            LastName = "Baldassarre",
            Phone = "3333333333"
        };
        
        customerConfiguration?.Invoke(customer);

        var appointment = new Appointment
        {
            Id = appointmentId,
            Customer = customer,
            Date = DateTime.UtcNow.AddDays(2),
            Status = AppointmentStatus.Scheduled
        };

        customer.Appointments = [appointment];

        return appointment;
    }
}