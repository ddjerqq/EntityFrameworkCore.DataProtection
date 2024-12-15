using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.DataProtection.ValueConverters;

/// <summary>
/// Value converter for string properties that uses data protection to encrypt and decrypt the value.
/// </summary>
public sealed class StringDataProtectionValueConverter : ValueConverter<string, string>
{
    /// <summary>
    /// Initializes a new instance of <see cref="StringDataProtectionValueConverter"/>.
    /// </summary>
    /// <param name="protector">the data protector used to protect and unprotect the data</param>
    /// <param name="internalConverter">the internal value converter to use</param>
    public StringDataProtectionValueConverter(IDataProtector protector, ValueConverter? internalConverter = null) : base(
        to => internalConverter == null ? protector.Protect(to) : protector.Protect((string)internalConverter.ConvertToProvider(to)!),
        from => internalConverter == null ? protector.Unprotect(from) : (string)internalConverter.ConvertFromProvider(protector.Unprotect(from))!)
    {
        if (internalConverter is null) return;

        var genericArguments = internalConverter.GetType().GetGenericArguments();
        var convertTo = genericArguments[1];

        if (convertTo != typeof(string))
            throw new InvalidOperationException("The internal value converter must convert your type to a string to be used as an intermediary converter.");
    }
}