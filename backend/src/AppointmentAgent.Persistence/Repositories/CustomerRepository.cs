using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAgent.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CustomerRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .Include(customer => customer.Appointments)
            .FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
    }
}