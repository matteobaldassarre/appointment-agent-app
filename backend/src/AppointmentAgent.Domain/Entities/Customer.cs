namespace AppointmentAgent.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; } // Generated MD5 Guid from FirstName, LastName and Phone
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Phone { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = [];
}