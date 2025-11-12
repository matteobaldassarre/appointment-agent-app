using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAgent.Persistence.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        await _dbContext.Appointments.AddAsync(appointment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Appointments
            .Include(appointment => appointment.Customer)
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Appointments
            .Include(appointment => appointment.Customer)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Update(appointment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Remove(appointment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}