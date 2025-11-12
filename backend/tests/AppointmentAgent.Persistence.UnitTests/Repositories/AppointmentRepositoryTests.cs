using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Entities.Enums;
using AppointmentAgent.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAgent.Persistence.UnitTests.Repositories;

[TestFixture]
public class AppointmentRepositoryTests : PersistenceTestsBase
{
    private AppointmentRepository _appointmentRepository = null!;

    [SetUp]
    public void SetUpTests()
    {
        _appointmentRepository = new AppointmentRepository(SetupDbContext);
    }

    [Test]
    public async Task AddAsync_WhenInvoked_AddsAppointmentToDatabase()
    {
        // Arrange
        var appointment = CreateTestAppointment();

        // Act
        await _appointmentRepository.AddAsync(appointment, CancellationToken);

        // Assert
        var storedAppointment = await AssertDbContext.Appointments
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == appointment.Id, CancellationToken);

        storedAppointment
            .Should()
            .BeEquivalentTo(
                appointment,
                options => options.IgnoringCyclicReferences()
            );
    }

    [Test]
    public async Task GetByIdAsync_WhenAppointmentExists_ReturnsAppointment()
    {
        // Arrange
        var appointment = CreateTestAppointment();
        
        await ArrangeDbContext.Appointments.AddAsync(appointment, CancellationToken);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken);

        // Act
        var storedAppointment = await _appointmentRepository.GetByIdAsync(appointment.Id, CancellationToken);

        // Assert
        storedAppointment
            .Should()
            .BeEquivalentTo(
                appointment,
                options => options.IgnoringCyclicReferences()
            );
    }

    [Test]
    public async Task GetAllAsync_WhenInvoked_ReturnsAllAppointments()
    {
        // Arrange
        var appointment1 = CreateTestAppointment();
        var appointment2 = CreateTestAppointment(
            customerFirstName: "Giulia", 
            customerLastName: "Baldassarre", 
            customerPhone: "9999999999"
        );
        
        await ArrangeDbContext.Appointments.AddRangeAsync([appointment1, appointment2], CancellationToken);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken);

        // Act
        var storedAppointments = (await _appointmentRepository.GetAllAsync(CancellationToken)).ToList();

        // Assert
        storedAppointments
            .Should()
            .BeEquivalentTo(
                [appointment1, appointment2],
                options => options
                    .WithoutStrictOrdering()
                    .IgnoringCyclicReferences()
            );
    }

    [Test]
    public async Task UpdateAsync_WhenInvoked_UpdatesAppointment()
    {
        // Arrange
        var appointment = CreateTestAppointment();
        
        await ArrangeDbContext.Appointments.AddAsync(appointment, CancellationToken);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken);

        appointment.Status = AppointmentStatus.Cancelled;
        
        // Act
        await _appointmentRepository.UpdateAsync(appointment, CancellationToken);

        // Assert
        var updatedAppointment = await AssertDbContext.Appointments
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == appointment.Id, CancellationToken);

        updatedAppointment
            .Should()
            .BeEquivalentTo(
                appointment,
                options => options.IgnoringCyclicReferences()
            );
    }

    [Test]
    public async Task DeleteAsync_WhenInvoked_RemovesAppointment()
    {
        // Arrange
        var appointment = CreateTestAppointment();
        
        await ArrangeDbContext.Appointments.AddAsync(appointment, CancellationToken);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken);

        // Act
        await _appointmentRepository.DeleteAsync(appointment, CancellationToken);

        // Assert
        var deletedAppointment = await AssertDbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointment.Id, CancellationToken);

        deletedAppointment
            .Should()
            .BeNull();
    }

    // Private Methods
    private static Appointment CreateTestAppointment(
        string customerFirstName = "Matteo", 
        string customerLastName = "Baldassarre", 
        string customerPhone = "3333333333"
    )
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = customerFirstName,
            LastName = customerLastName,
            Phone = customerPhone
        };

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
}
