using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Entities.Enums;
using AppointmentAgent.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAgent.Persistence.UnitTests.Repositories;

[TestFixture]
public class CustomerRepositoryTests : PersistenceTestsBase
{
    private CustomerRepository _customerRepository = null!;

    [SetUp]
    public void SetUpTests()
    {
        _customerRepository = new CustomerRepository(SetupDbContext);
    }
    
    [Test]
    public async Task AddAsync_WhenInvoked_AddsCustomerToDatabase()
    {
        // Arrange
        var customer = CreateTestCustomer();
        
        // Act
        await _customerRepository.AddAsync(customer, CancellationToken);
        
        // Assert
        var storedCustomer = await AssertDbContext.Customers
            .Include(c => c.Appointments)
            .FirstOrDefaultAsync(x => x.Id == customer.Id, CancellationToken);

        storedCustomer
            .Should()
            .BeEquivalentTo(
                customer, 
                options => options.IgnoringCyclicReferences()
            );
    }

    [Test]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomer()
    {
        // Arrange
        var customer = CreateTestCustomer();
        
        await ArrangeDbContext.Customers.AddAsync(customer, CancellationToken);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken);

        // Act
        var storedCustomer = await _customerRepository.GetByIdAsync(customer.Id, CancellationToken);

        // Assert
        storedCustomer
            .Should()
            .BeEquivalentTo(
                customer, 
                options => options.IgnoringCyclicReferences()
            );
    }

    [Test]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Act
        var storedCustomer = await _customerRepository.GetByIdAsync(Guid.NewGuid(), CancellationToken);

        // Assert
        storedCustomer
            .Should()
            .BeNull();
    }
    
    // Private Methods
    private static Customer CreateTestCustomer()
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

        return customer;
    }
}