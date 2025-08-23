using ClearBudget.Database.Entities.Settings;
using ClearBudget.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Configurations.Settings;

public class SettingEntityTypeConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ConfigureBaseProperties();

        builder.Property(x => x.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsEncrypted()
            .IsRequired();
    }
}
