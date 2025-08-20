using ClearBudget.Infrastructure.Enums.Encryption;

namespace ClearBudget.Infrastructure.Attributes;

/// <summary>
///     Specifies that the data field value should be encrypted.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EncryptedAttribute(StorageFormat format = StorageFormat.Default) : Attribute
{
    /// <summary>
    ///     Returns the storage format for the database value.
    /// </summary>
    public StorageFormat Format { get; } = format;
}