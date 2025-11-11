using AppointmentAgent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentAgent.Persistence.EntityConfigurations;

public class AppointmentEntityConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");
        
        builder
            .HasOne(e => e.Customer)
            .WithMany(e => e.Appointments)
            .HasForeignKey(e => e.Customer.Id)
            .HasPrincipalKey(e => e.Id);
    }
}