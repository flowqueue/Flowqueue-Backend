using System.Security.Cryptography;
using System.Text;

namespace Flowqueue_Backend.IAM.Application.Internal.CommandServices;

internal static class PasswordHashingService
{
    public static string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password.Trim())));
    }
}
