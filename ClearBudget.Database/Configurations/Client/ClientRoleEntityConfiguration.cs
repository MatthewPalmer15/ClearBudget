using ClearBudget.Database.Entities.Client;
using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Client;

internal class ClientRoleEntityConfiguration : IEntityTypeConfiguration<ClientRole>
{
    public void Configure(EntityTypeBuilder<ClientRole> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}