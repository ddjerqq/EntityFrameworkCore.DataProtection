using System.Security.Cryptography;
using System.Text;

namespace EntityFrameworkCore.DataProtection.Extensions;

/// <summary>
/// Provides extensions for the <see cref="string"/> type.
/// </summary>
internal static class StringExt
{
    internal const string EnvKey = "EFCORE_DATA_PROTECTION__HASHING_SALT";

    internal static string HmacSha256Hash(this string value)
    {
        var salt =
            Environment.GetEnvironmentVariable(EnvKey)
            ?? throw new InvalidOperationException($"{EnvKey} is not present in the environment, please set it to a strong value and keep it safe, otherwise querying will not work.");

        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var payloadBytes = Encoding.UTF8.GetBytes(value);
        var hash = HMACSHA256.HashData(saltBytes, payloadBytes);
        var hexDigest = Convert.ToHexString(hash);
        return hexDigest.ToLower();
    }
}