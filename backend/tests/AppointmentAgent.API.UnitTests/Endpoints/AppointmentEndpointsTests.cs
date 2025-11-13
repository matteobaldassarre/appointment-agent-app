using AppointmentAgent.API.Endpoints;
using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Services;
using AppointmentAgent.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AppointmentAgent.API.UnitTests.Endpoints;

[TestFixture]
public class AppointmentEndpointsTests
{
    private Mock<IAppointmentService> _appointmentServiceMock = null!;
    private Mock<IConfiguration> _configurationMock = null!;
    private const string AgentApiKey = "secret-agent-api-key";

    [SetUp]
    public void SetUp()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>(MockBehavior.Strict);
        _configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
    }

    [Test]
    public async Task GetAllAppointments_WhenInvoked_ReturnsAllAppointments()
    {
        // Arrange
        var appointment1 = AppointmentTestFactory.CreateAppointment();
        var appointment2 = AppointmentTestFactory.CreateAppointment(appointmentConfiguration: a => a.Customer.FirstName = "Giulia");

        var expectedAppointments = new[] { appointment1, appointment2 };

        _appointmentServiceMock
            .Setup(service => service.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAppointments);

        // Act
        var response = await AppointmentEndpoints.GetAllAppointments(
            _appointmentServiceMock.Object,
            CancellationToken.None
        );

        // Assert
        response
            .Should()
            .BeOfType<Ok<IEnumerable<Appointment>>>();
        
        response.Value
            .Should()
            .BeEquivalentTo(
                expectedAppointments, 
                options => options.WithoutStrictOrdering()
            );
    }

    [Test]
    public async Task GetAppointment_WhenAppointmentExists_ReturnsOkWithAppointment()
    {
        // Arrange
        var appointment = AppointmentTestFactory.CreateAppointment();

        _appointmentServiceMock
            .Setup(service => service.GetByIdAsync(appointment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var response = await AppointmentEndpoints.GetAppointment(
            appointment.Id,
            _appointmentServiceMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<Ok<Appointment>>();
        
        response.Result
            .As<Ok<Appointment>>().Value
            .Should()
            .BeEquivalentTo(appointment);
    }

    [Test]
    public async Task GetAppointment_WhenAppointmentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        _appointmentServiceMock
            .Setup(service => service.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?) null);

        // Act
        var response = await AppointmentEndpoints.GetAppointment(
            nonExistentId,
            _appointmentServiceMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<NotFound>();
    }

    [Test]
    public async Task CreateAppointment_WhenApiKeyIsValid_CreatesAppointmentAndReturnsCreated()
    {
        // Arrange
        var newAppointment = AppointmentTestFactory.CreateAppointment();

        _configurationMock
            .Setup(configuration => configuration["AgentApiKey"])
            .Returns(AgentApiKey);
        
        _appointmentServiceMock
            .Setup(service => service.CreateAsync(newAppointment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newAppointment);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["x-agent-api-key"] = AgentApiKey;

        // Act
        var response = await AppointmentEndpoints.CreateAppointment(
            httpContext.Request,
            newAppointment,
            _appointmentServiceMock.Object,
            _configurationMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<Created<Appointment>>();
        
        var createdResponse = response.Result.As<Created<Appointment>>();

        createdResponse.Location
            .Should()
            .Be($"/appointments/{newAppointment.Id}");
        
        createdResponse.Value
            .Should()
            .BeEquivalentTo(newAppointment);

        _appointmentServiceMock.Verify(
            service => service.CreateAsync(newAppointment, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task CreateAppointment_WhenApiKeyIsMissing_ReturnsUnauthorized()
    {
        // Arrange
        var newAppointment = AppointmentTestFactory.CreateAppointment();

        _configurationMock
            .Setup(configuration => configuration["AgentApiKey"])
            .Returns(AgentApiKey);

        var httpContext = new DefaultHttpContext();

        // Act
        var response = await AppointmentEndpoints.CreateAppointment(
            httpContext.Request,
            newAppointment,
            _appointmentServiceMock.Object,
            _configurationMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<UnauthorizedHttpResult>();
        
        _appointmentServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task CreateAppointment_WhenApiKeyIsIncorrect_ReturnsUnauthorized()
    {
        // Arrange
        var newAppointment = AppointmentTestFactory.CreateAppointment();

        _configurationMock
            .Setup(configuration => configuration["AgentApiKey"])
            .Returns(AgentApiKey);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["x-agent-api-key"] = "wrong-key";

        // Act
        var response = await AppointmentEndpoints.CreateAppointment(
            httpContext.Request,
            newAppointment,
            _appointmentServiceMock.Object,
            _configurationMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<UnauthorizedHttpResult>();
        
        _appointmentServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task UpdateAppointment_WhenUpdateSucceeds_ReturnsNoContent()
    {
        // Arrange
        var updatingAppointment = AppointmentTestFactory.CreateAppointment();

        _appointmentServiceMock
            .Setup(service => service.TryUpdateAsync(updatingAppointment.Id, updatingAppointment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var response = await AppointmentEndpoints.UpdateAppointment(
            updatingAppointment.Id,
            updatingAppointment,
            _appointmentServiceMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<NoContent>();
    }

    [Test]
    public async Task UpdateAppointment_WhenUpdateFails_ReturnsNotFound()
    {
        // Arrange
        var updatingAppointment = AppointmentTestFactory.CreateAppointment();

        _appointmentServiceMock
            .Setup(service => service.TryUpdateAsync(updatingAppointment.Id, updatingAppointment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await AppointmentEndpoints.UpdateAppointment(
            updatingAppointment.Id,
            updatingAppointment,
            _appointmentServiceMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<NotFound>();
    }

    [Test]
    public async Task DeleteAppointment_WhenDeleteSucceeds_ReturnsNoContent()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentServiceMock
            .Setup(service => service.TryDeleteAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var response = await AppointmentEndpoints.DeleteAppointment(
            appointmentId,
            _appointmentServiceMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<NoContent>();
    }

    [Test]
    public async Task DeleteAppointment_WhenDeleteFails_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentServiceMock
            .Setup(service => service.TryDeleteAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await AppointmentEndpoints.DeleteAppointment(
            appointmentId,
            _appointmentServiceMock.Object,
            CancellationToken.None
        );

        // Assert
        response.Result
            .Should()
            .BeOfType<NotFound>();
    }
}