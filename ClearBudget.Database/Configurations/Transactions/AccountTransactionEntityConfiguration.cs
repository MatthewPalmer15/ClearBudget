using ClearBudget.Database.Entities.Transactions;
using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Transactions;

internal class AccountTransactionEntityConfiguration : IEntityTypeConfiguration<AccountTransaction>
{
    public void Configure(EntityTypeBuilder<AccountTransaction> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TransactionDate)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Amount)
            .IsRequired();

        builder.Property(x => x.Recurring)
            .IsRequired();

        builder.Property(x => x.RecurringCustomDays)
            .IsRequired(false);

        builder.Property(x => x.CategoryId)
            .IsRequired(false);

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}