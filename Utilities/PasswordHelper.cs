using System.Security.Cryptography;
using System.Text;

namespace Vikalp.Utilities;

public static class PasswordHelper
{
    public static string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var saltedPassword = password + salt;
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
        return Convert.ToBase64String(hashedBytes);
    }

    public static (string hash, string salt) HashPassword(string password)
    {
        var salt = GenerateSalt();
        var hash = HashPassword(password, salt);
        return (hash, salt);
    }

    public static string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    public static bool VerifyPassword(string password, string hash, string salt)
    {
        var computedHash = HashPassword(password, salt);
        return computedHash == hash;
    }
}
