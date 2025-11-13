using AppointmentAgent.Domain.Entities;

namespace AppointmentAgent.Domain.Services;

public interface IAppointmentService
{
    Task<Appointment> CreateAsync(Appointment appointment, CancellationToken cancellationToken);
    Task<bool> TryUpdateAsync(Guid id, Appointment updatingAppointment, CancellationToken cancellationToken);
    Task<bool> TryDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken);
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}