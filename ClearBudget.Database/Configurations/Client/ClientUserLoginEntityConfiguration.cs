using ClearBudget.Database.Entities.Client;

using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Client;

internal class ClientUserLoginEntityConfiguration : IEntityTypeConfiguration<ClientUserLogin>
{
    public void Configure(EntityTypeBuilder<ClientUserLogin> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(x => x.ClientUserId)
            .IsRequired();

        builder.HasOne(x => x.ClientUser)
            .WithMany()
            .HasForeignKey(x => x.ClientUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Provider)
            .IsRequired();

        builder.Property(x => x.ProviderKey)
            .IsRequired();

        builder.Property(x => x.ProviderDisplayName)
            .IsRequired(false);
    }
}