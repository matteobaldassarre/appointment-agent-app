using AppointmentAgent.Domain.Entities.Enums;

namespace AppointmentAgent.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public required Customer Customer { get; set; }
    public DateTime Date { get; set; }
    public AppointmentStatus Status { get; set; }
}