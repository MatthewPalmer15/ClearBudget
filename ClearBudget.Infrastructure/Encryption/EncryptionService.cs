using ClearBudget.Infrastructure.Encryption.Providers.Abstract;
using ClearBudget.Infrastructure.Encryption.Providers.Concrete;
using System.Text;

namespace ClearBudget.Infrastructure.Encryption;

public class EncryptionService : IEncryptionService
{
    public EncryptionService()
    {

        var keyBytes = Convert.FromBase64String("qN5Iu+oZ6e4Bz6sFv3Nz2WE+9m58ErpW8+CtNDGnqDQ=");
        var ivBytes = Convert.FromBase64String("vA+boKe69Tw+zM22FXTe2Q==");

        EncryptionProvider = new AesProvider(keyBytes, ivBytes);
    }


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
    public string Encrypt(string text)
    {
        var textBytes = Encoding.UTF8.GetBytes(text);
        var encryptedBytes = EncryptionProvider.Encrypt(textBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    ///     Decrypts the specified text.
    /// </summary>
    /// <param name="encryptedText">The text to decrypt, in Base64 format.</param>
    /// <returns>The decrypted text.</returns>
    public string Decrypt(string encryptedText)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedText);
        var decryptedBytes = EncryptionProvider.Decrypt(encryptedBytes);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}