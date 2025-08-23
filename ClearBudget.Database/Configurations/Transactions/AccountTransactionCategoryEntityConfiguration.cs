using ClearBudget.Database.Entities.Transactions;
using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Transactions;

internal class AccountTransactionCategoryEntityConfiguration : IEntityTypeConfiguration<AccountTransactionCategory>
{
    public void Configure(EntityTypeBuilder<AccountTransactionCategory> builder)
    {
        builder.ConfigureBaseProperties();
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();
    }
}