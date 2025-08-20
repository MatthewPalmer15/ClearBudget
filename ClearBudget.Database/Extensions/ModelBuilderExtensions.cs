using ClearBudget.Database.Internal.Encryption;
using ClearBudget.Infrastructure.Encryption.Providers.Abstract;
using ClearBudget.Infrastructure.Enums.Encryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClearBudget.Database.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies all the configuration from all the assemblies in the project to the ModelBuilder.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder to apply the configurations to.</param>
    /// <returns>The same ModelBuilder instance so that multiple configuration calls can be chained.</returns>
    public static ModelBuilder ApplyConfigurations(this ModelBuilder modelBuilder)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies) modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        return modelBuilder;
    }

    internal static ModelBuilder UseEncryption(this ModelBuilder modelBuilder, IEncryptionProvider encryptionProvider)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(encryptionProvider);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var encryptedProperties = GetEntityEncryptedProperties(entityType);

            foreach (var encryptedProperty in encryptedProperties)
            {
#pragma warning disable EF1001 // Internal EF Core API usage.
                if (encryptedProperty.Property.FindAnnotation(CoreAnnotationNames.ValueConverter) is not null) continue;
#pragma warning restore EF1001 // Internal EF Core API usage.

                var converter = GetValueConverter(encryptedProperty.Property.ClrType, encryptionProvider,
                    encryptedProperty.StorageFormat);

                if (converter != null) encryptedProperty.Property.SetValueConverter(converter);
            }
        }

        return modelBuilder;
    }

    private static ValueConverter GetValueConverter(Type propertyType, IEncryptionProvider encryptionProvider,
        StorageFormat storageFormat)
    {
        if (propertyType == typeof(string))
            return storageFormat switch
            {
                StorageFormat.Default or StorageFormat.Base64 => new EncryptionConverter<string, string>(
                    encryptionProvider, StorageFormat.Base64),
                StorageFormat.Binary => new EncryptionConverter<string, byte[]>(encryptionProvider,
                    StorageFormat.Binary),
                _ => throw new NotImplementedException()
            };

        if (propertyType == typeof(byte[]))
            return storageFormat switch
            {
                StorageFormat.Default or StorageFormat.Binary => new EncryptionConverter<byte[], byte[]>(
                    encryptionProvider, StorageFormat.Binary),
                StorageFormat.Base64 => new EncryptionConverter<byte[], string>(encryptionProvider,
                    StorageFormat.Base64),
                _ => throw new NotImplementedException()
            };

        throw new NotImplementedException($"Type {propertyType.Name} does not support encryption.");
    }

    private static IEnumerable<EncryptedProperty> GetEntityEncryptedProperties(IMutableEntityType entity)
    {
        return entity.GetProperties()
            .Select(x => EncryptedProperty.Create(x))
            .Where(x => x is not null);
    }
}