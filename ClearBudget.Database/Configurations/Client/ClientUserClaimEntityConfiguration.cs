using ClearBudget.Database.Entities.Client;
using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Client;

internal class ClientUserClaimEntityConfiguration : IEntityTypeConfiguration<ClientUserClaim>
{
    public void Configure(EntityTypeBuilder<ClientUserClaim> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(x => x.ClientUserId)
            .IsRequired();

        builder.HasOne(x => x.ClientUser)
            .WithMany()
            .HasForeignKey(x => x.ClientUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Type)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(400)
            .IsRequired();

        builder.HasIndex(x => new { x.ClientUserId, x.Type }).IsUnique();
    }
}