using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Entities.Enums;

namespace AppointmentAgent.TestUtils;

public static class CustomerTestFactory
{
    public static Customer CreateCustomer(Action<Customer>? customerConfiguration = null)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Matteo",
            LastName = "Baldassarre",
            Phone = "3333333333"
        };

        customer.Appointments =
        [
            new Appointment
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                Date = DateTime.UtcNow.AddDays(-1),
                Status = AppointmentStatus.Fulfilled
            },
            new Appointment
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                Date = DateTime.UtcNow.AddDays(1),
                Status = AppointmentStatus.Scheduled
            }
        ];
        
        customerConfiguration?.Invoke(customer);
        
        return customer;
    }
}