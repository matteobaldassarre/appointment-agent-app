using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Entities.Enums;
using AppointmentAgent.Domain.Repositories;
using AppointmentAgent.Domain.Services;
using AppointmentAgent.TestUtils;
using FluentAssertions;
using Moq;

namespace AppointmentAgent.Domain.UnitTests.Services;

[TestFixture]
public class AppointmentServiceTests
{
    private Mock<IAppointmentRepository> _appointmentRepositoryMock = null!;
    private AppointmentService _appointmentService = null!;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void Setup()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>(MockBehavior.Strict);
        _appointmentService = new AppointmentService(_appointmentRepositoryMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Test]
    public void CreateAsync_WhenAppointmentIsNull_ThrowsException()
    {
        // Act && Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _appointmentService.CreateAsync(null!, _cancellationToken));
    }

    [Test]
    public async Task CreateAsync_WhenInvoked_AddsAndReturnsAppointment()
    {
        // Arrange
        var appointment = AppointmentTestFactory.CreateAppointment();

        _appointmentRepositoryMock
            .Setup(repository => repository.AddAsync(appointment, _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var createdAppointment = await _appointmentService.CreateAsync(appointment, _cancellationToken);

        // Assert
        createdAppointment
            .Should()
            .BeEquivalentTo(appointment);
        
        _appointmentRepositoryMock.Verify(
            repository => repository.AddAsync(appointment, _cancellationToken), 
            Times.Once
        );
    }

    [Test]
    public void TryUpdateAsync_WhenAppointmentIsNull_ThrowsException()
    {
        // Act && Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            () => _appointmentService.TryUpdateAsync(Guid.NewGuid(), null!, _cancellationToken)
        );
    }

    [Test]
    public async Task TryUpdateAsync_WhenAppointmentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var appointment = AppointmentTestFactory.CreateAppointment();

        _appointmentRepositoryMock
            .Setup(repository => repository.GetByIdAsync(appointment.Id, _cancellationToken))
            .ReturnsAsync((Appointment?) null);

        // Act
        var updateResult = await _appointmentService.TryUpdateAsync(appointment.Id, appointment, _cancellationToken);

        // Assert
        Assert.That(updateResult, Is.False);
        
        _appointmentRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Appointment>(), _cancellationToken), 
            Times.Never
        );
    }

    [Test]
    public async Task TryUpdateAsync_WhenAppointmentExists_UpdatesAndReturnsTrue()
    {
        // Arrange
        var existingAppointment = AppointmentTestFactory.CreateAppointment();
        
        var updatingAppointment = AppointmentTestFactory.CreateAppointment(
            appointmentConfiguration: appointment =>
            {
                appointment.Id = existingAppointment.Id;
                appointment.Customer.Id = existingAppointment.Customer.Id;
            }
        );
        updatingAppointment.Status = AppointmentStatus.Cancelled;
        updatingAppointment.Date = DateTime.UtcNow.AddDays(1);

        _appointmentRepositoryMock
            .Setup(repository => repository.GetByIdAsync(existingAppointment.Id, _cancellationToken))
            .ReturnsAsync(existingAppointment);
        
        _appointmentRepositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Appointment>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var updateResult = await _appointmentService.TryUpdateAsync(updatingAppointment.Id, updatingAppointment, _cancellationToken);

        // Assert
        Assert.That(updateResult, Is.True);

        existingAppointment
            .Should()
            .BeEquivalentTo(updatingAppointment);
        
        _appointmentRepositoryMock
            .Verify(
                repository => repository.UpdateAsync(existingAppointment, _cancellationToken), 
                Times.Once
            ); 
    }

    [Test]
    public async Task TryDeleteAsync_WhenAppointmentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        _appointmentRepositoryMock
            .Setup(repository => repository.GetByIdAsync(id, _cancellationToken))
            .ReturnsAsync((Appointment?) null);

        // Act
        var deleteResult = await _appointmentService.TryDeleteAsync(id, _cancellationToken);

        // Assert
        Assert.That(deleteResult, Is.False);
        
        _appointmentRepositoryMock.Verify(
            repository => repository.DeleteAsync(It.IsAny<Appointment>(), _cancellationToken), 
            Times.Never
        );
    }

    [Test]
    public async Task TryDeleteAsync_WhenAppointmentExists_DeletesAppointmentAndReturnsTrue()
    {
        // Arrange
        var appointment = AppointmentTestFactory.CreateAppointment();
        
        _appointmentRepositoryMock
            .Setup(repository => repository.GetByIdAsync(appointment.Id, _cancellationToken))
            .ReturnsAsync(appointment);
        
        _appointmentRepositoryMock
            .Setup(repository => repository.DeleteAsync(It.IsAny<Appointment>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var deleteResult = await _appointmentService.TryDeleteAsync(appointment.Id, _cancellationToken);

        // Assert
        Assert.That(deleteResult, Is.True);
        
        _appointmentRepositoryMock.Verify(
            repository => repository.DeleteAsync(appointment, _cancellationToken), 
            Times.Once
        );
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllAppointments()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            AppointmentTestFactory.CreateAppointment(),
            AppointmentTestFactory.CreateAppointment(
                appointmentConfiguration: appointment =>
                {
                    appointment.Customer.FirstName = "Giulia";
                    appointment.Customer.LastName = "Baldassarre";
                    appointment.Customer.Phone = "9999999999";
                }
            )
        };

        _appointmentRepositoryMock
            .Setup(repository => repository.GetAllAsync(_cancellationToken))
            .ReturnsAsync(appointments);

        // Act
        var storedAppointments = await _appointmentService.GetAllAsync(_cancellationToken);

        // Assert
        storedAppointments
            .Should()
            .BeEquivalentTo(appointments);
    }

    [Test]
    public async Task GetByIdAsync_WhenAppointmentExists_ReturnsAppointment()
    {
        // Arrange
        var appointment = AppointmentTestFactory.CreateAppointment();

        _appointmentRepositoryMock
            .Setup(repository => repository.GetByIdAsync(appointment.Id, _cancellationToken))
            .ReturnsAsync(appointment);

        // Act
        var storedAppointment = await _appointmentService.GetByIdAsync(appointment.Id, _cancellationToken);

        // Assert
        storedAppointment
            .Should()
            .BeEquivalentTo(appointment);
    }

    [Test]
    public async Task GetByIdAsync_WhenAppointmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync((Appointment?) null);

        // Act
        var storedAppointment = await _appointmentService.GetByIdAsync(Guid.NewGuid(), _cancellationToken);

        // Assert
        Assert.That(storedAppointment, Is.Null);
    }
}