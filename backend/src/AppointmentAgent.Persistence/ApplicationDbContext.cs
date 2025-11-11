using AppointmentAgent.Domain.Entities;
using AppointmentAgent.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAgent.Persistence;

public class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Customer> Customers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentEntityConfiguration());
    }
}