using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Entities.Enums;

namespace AppointmentAgent.TestUtils;

public static class AppointmentTestFactory
{
    public static Appointment CreateAppointment(Action<Appointment>? appointmentConfiguration = null)
    {
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            Customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Matteo",
                LastName = "Baldassarre",
                Phone = "3333333333"
            },
            Date = DateTime.UtcNow.AddDays(2),
            Status = AppointmentStatus.Scheduled
        };
        
        appointmentConfiguration?.Invoke(appointment);

        return appointment;
    }
}