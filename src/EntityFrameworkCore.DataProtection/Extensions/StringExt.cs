using System.Security.Cryptography;
using System.Text;

namespace EntityFrameworkCore.DataProtection.Extensions;

/// <summary>
/// Provides extensions for the <see cref="string"/> type.
/// </summary>
internal static class StringExt
{
    internal static string Sha256Hash(this string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        var hexDigest = Convert.ToHexString(hash);
        return hexDigest.ToLower();
    }
}