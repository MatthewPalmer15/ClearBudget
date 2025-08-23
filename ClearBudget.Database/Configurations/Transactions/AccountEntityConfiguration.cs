using ClearBudget.Database.Entities.Transactions;
using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Transactions;

internal class AccountEntityConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(e => e.ClientUserId)
            .IsRequired();

        builder.HasOne(o => o.ClientUser)
            .WithMany()
            .HasForeignKey(o => o.ClientUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.DateCreated)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(x => x.DateClosed)
            .IsRequired(false);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.InterestRate)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}