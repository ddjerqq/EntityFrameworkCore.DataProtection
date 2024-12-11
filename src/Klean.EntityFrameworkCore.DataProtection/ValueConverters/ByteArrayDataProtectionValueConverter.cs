using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.DataProtection.ValueConverters;

/// <summary>
/// Value converter for string properties that uses data protection to encrypt and decrypt the value.
/// </summary>
public sealed class ByteArrayDataProtectionValueConverter : ValueConverter<byte[], byte[]>
{
    /// <summary>
    /// Initializes a new instance of <see cref="ByteArrayDataProtectionValueConverter"/>.
    /// </summary>
    /// <param name="protector"></param>
    public ByteArrayDataProtectionValueConverter(IDataProtector protector) : base(
        to => protector.Protect(to),
        from => protector.Unprotect(from))
    {
    }
}
