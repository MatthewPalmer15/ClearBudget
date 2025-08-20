using System.Security.Cryptography;

namespace ClearBudget.Infrastructure.Services.Hash;

public interface IHashService
{
    string Hash(string input);
    bool Verify(string input, string hash);
}

public class HashService : IHashService
{
    private const int SaltSize = 16; // 128-bit
    private const int HashSize = 32; // 256-bit
    private const int Iterations = 100_000;

    public string Hash(string input)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(input, salt, Iterations, HashAlgorithmName.SHA512);
        var hash = pbkdf2.GetBytes(HashSize);

        // Format: {iterations}.{base64 salt}.{base64 hash}
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string input, string hash)
    {
        try
        {
            var parts = hash.Split('.', 3);
            if (parts.Length != 3) return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var storedHash = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(input, salt, iterations, HashAlgorithmName.SHA512);
            var computedHash = pbkdf2.GetBytes(storedHash.Length);

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }
}