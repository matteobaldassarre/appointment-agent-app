using AppointmentAgent.Domain.Entities;

namespace AppointmentAgent.Persistence.Repositories;

public interface IAppointmentRepository
{
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken);
    Task DeleteAsync(Appointment appointment, CancellationToken cancellationToken);
}