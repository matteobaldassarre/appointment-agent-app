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
        
        customerConfiguration?.Invoke(customer);
        
        return customer;
    }
}