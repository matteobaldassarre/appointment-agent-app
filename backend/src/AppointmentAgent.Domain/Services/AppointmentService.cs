using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Repositories;

namespace AppointmentAgent.Domain.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Appointment> CreateAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        if (appointment is null)
            throw new ArgumentNullException(nameof(appointment));

        await _appointmentRepository.AddAsync(appointment, cancellationToken);

        return appointment;
    }

    public async Task<bool> TryUpdateAsync(Appointment updatingAppointment, CancellationToken cancellationToken)
    {
        if (updatingAppointment is null) 
            throw new ArgumentNullException(nameof(updatingAppointment));

        var existingAppointment = await _appointmentRepository.GetByIdAsync(updatingAppointment.Id, cancellationToken);

        if (existingAppointment is null)
            return false;

        existingAppointment.Customer = updatingAppointment.Customer;
        existingAppointment.Date = updatingAppointment.Date;
        existingAppointment.Status = updatingAppointment.Status;

        await _appointmentRepository.UpdateAsync(existingAppointment, cancellationToken);
        
        return true;
    }

    public async Task<bool> TryDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingAppointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);

        if (existingAppointment is null) 
            return false;

        await _appointmentRepository.DeleteAsync(existingAppointment, cancellationToken);
        
        return true;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _appointmentRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _appointmentRepository.GetByIdAsync(id, cancellationToken);
    }
}