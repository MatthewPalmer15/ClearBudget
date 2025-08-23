using ClearBudget.Database.Entities.Client;
using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Client;

internal class ClientUserEntityConfiguration : IEntityTypeConfiguration<ClientUser>
{
    public void Configure(EntityTypeBuilder<ClientUser> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(x => x.DateCreated)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(x => x.Forename)
            .HasMaxLength(100)
            .IsEncrypted()
            .IsRequired();

        builder.Property(x => x.Surname)
            .HasMaxLength(100)
            .IsEncrypted()
            .IsRequired();

        builder.Property(x => x.EmailAddress)
            .IsEncrypted()
            .IsRequired();

        builder.HasIndex(x => x.EmailAddress)
            .IsUnique();

        builder.Property(x => x.Password)
            .IsRequired();

        builder.Property(x => x.TwoFactorEnabled)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.TwoFactorKey)
            .IsRequired(false);

        builder.Property(x => x.MaxLoginAttempts)
            .HasDefaultValue(5)
            .IsRequired();

        builder.Property(x => x.FailedLoginAttempts)
            .HasDefaultValue(0)
            .IsRequired();
    }
}