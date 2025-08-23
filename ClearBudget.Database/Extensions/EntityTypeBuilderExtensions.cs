using ClearBudget.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureBaseProperties<TEntity>(this EntityTypeBuilder<TEntity> entity)
        where TEntity : class, IBaseEntity<Guid>
    {
        entity.ToTable($"tbl_{typeof(TEntity).Name}");

        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Id)
            .IsUnique();

        entity.Property(e => e.Id)
            .HasColumnName($"{typeof(TEntity).Name}Id")
            .HasColumnOrder(1)
            .ValueGeneratedOnAdd()
            .IsRequired();

        entity.Property(e => e.Deleted)
            .HasDefaultValue(false)
            .IsRequired();

        entity.HasQueryFilter(e => !e.Deleted);
        return entity;
    }
}