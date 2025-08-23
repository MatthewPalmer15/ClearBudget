using ClearBudget.Database.Entities.Client;

using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Client;

internal class ClientUserRoleEntityConfiguration : IEntityTypeConfiguration<ClientUserRole>
{
    public void Configure(EntityTypeBuilder<ClientUserRole> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(x => x.ClientUserId)
            .IsRequired();

        builder.HasOne(x => x.ClientUser)
            .WithMany()
            .HasForeignKey(x => x.ClientUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ClientRoleId)
            .IsRequired();

        builder.HasOne(x => x.ClientRole)
            .WithMany()
            .HasForeignKey(x => x.ClientRoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}