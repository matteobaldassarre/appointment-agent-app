using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Entities.Enums;
using AppointmentAgent.Persistence.Repositories;
using AppointmentAgent.TestUtils;
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
        var appointment = AppointmentTestFactory.CreateAppointment();

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
        var appointment = AppointmentTestFactory.CreateAppointment();
        
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
        var appointment1 = AppointmentTestFactory.CreateAppointment();
        var appointment2 = AppointmentTestFactory.CreateAppointment(
            customerConfiguration: customer =>
            {
                customer.FirstName = "Giulia";
                customer.LastName = "Baldassarre";
                customer.Phone = "9999999999";
            }
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
        var appointment = AppointmentTestFactory.CreateAppointment();
        
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
        var appointment = AppointmentTestFactory.CreateAppointment();
        
        await ArrangeDbContext.Appointments.AddAsync(appointment, CancellationToken);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken);

        // Act
        await _appointmentRepository.DeleteAsync(appointment, CancellationToken);

        // Assert
        var deletedAppointment = await AssertDbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointment.Id, CancellationToken);

        Assert.That(deletedAppointment, Is.Null);
    }
}
