using ClearBudget.Infrastructure.Encryption.Providers.Abstract;

namespace ClearBudget.Infrastructure.Encryption;

public interface IEncryptionService
{
    /// <summary>
    ///     Gets an encryption provider that uses the AES algorithm.
    /// </summary>
    /// <value>
    ///     An <see cref="IEncryptionProvider" /> that uses the AES algorithm.
    /// </value>
    public IEncryptionProvider EncryptionProvider { get; }

    /// <summary>
    ///     Encrypts the specified text.
    /// </summary>
    /// <param name="text">The text to encrypt.</param>
    /// <returns>A string that represents the encrypted text, in Base64 format.</returns>
    public string Encrypt(string text);

    /// <summary>
    ///     Decrypts the specified text.
    /// </summary>
    /// <param name="encryptedText">The text to decrypt, in Base64 format.</param>
    /// <returns>The decrypted text.</returns>
    public string Decrypt(string encryptedText);
}