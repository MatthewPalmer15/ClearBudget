using ClearBudget.Database.Internal.Encryption;
using ClearBudget.Infrastructure.Enums.Encryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearBudget.Database.Extensions;

/// <summary>
///     Provides extensions for the <see cref="PropertyBuilder" /> type.
/// </summary>
public static class PropertyBuilderExtensions
{
    public static PropertyBuilder IsEncrypted(this PropertyBuilder builder,
        StorageFormat storageFormat = StorageFormat.Default)
    {
        if (builder is null) throw new ArgumentNullException(nameof(builder));

        builder.HasAnnotation(EncryptionAnnotations.IsEncrypted, true);
        builder.HasAnnotation(EncryptionAnnotations.StorageFormat, storageFormat);

        return builder;
    }

    public static PropertyBuilder<TProperty> IsEncrypted<TProperty>(this PropertyBuilder<TProperty> builder,
        StorageFormat storageFormat = StorageFormat.Default)
    {
        if (builder is null) throw new ArgumentNullException(nameof(builder));

        builder.HasAnnotation(EncryptionAnnotations.IsEncrypted, true);
        builder.HasAnnotation(EncryptionAnnotations.StorageFormat, storageFormat);

        return builder;
    }
}