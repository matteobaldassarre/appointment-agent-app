using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Entities.Enums;
using AppointmentAgent.Persistence.Repositories;
using AppointmentAgent.TestUtils;
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
        var customer = CustomerTestFactory.CreateCustomer();
        
        // Act
        await _customerRepository.AddAsync(customer, CancellationToken);
        
        // Assert
        var storedCustomer = await AssertDbContext.Customers
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
        var customer = CustomerTestFactory.CreateCustomer();
        
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
        Assert.That(storedCustomer, Is.Null);
    }
}