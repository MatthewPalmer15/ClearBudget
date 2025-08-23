using ClearBudget.Database.Entities.Client;
using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Client;

internal class ClientRoleClaimEntityConfiguration : IEntityTypeConfiguration<ClientRoleClaim>
{
    public void Configure(EntityTypeBuilder<ClientRoleClaim> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(x => x.ClientRoleId)
            .IsRequired();

        builder.HasOne(x => x.ClientRole)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.ClientRoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Type)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(400)
            .IsRequired();

        builder.HasIndex(x => new { x.ClientRoleId, x.Type }).IsUnique();
    }
}