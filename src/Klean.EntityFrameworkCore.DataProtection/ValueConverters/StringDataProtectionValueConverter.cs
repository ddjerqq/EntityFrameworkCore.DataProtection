using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.DataProtection.ValueConverters;

internal sealed class StringDataProtectionValueConverter<T> : ValueConverter<T, string>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of <see cref="StringDataProtectionValueConverter{T}"/>.
    /// </summary>
    /// <param name="protector">the data protector used to protect and unprotect the data</param>
    /// <param name="internalConverter">the internal value converter to use</param>
    public StringDataProtectionValueConverter(IDataProtector protector, ValueConverter? internalConverter = null) : base(
        to => internalConverter == null ? protector.Protect((string)(object)to) : protector.Protect((string)internalConverter.ConvertToProvider(to)!),
        from => internalConverter == null ? (T)(object)protector.Unprotect(from) : (T)internalConverter.ConvertFromProvider(protector.Unprotect(from))!)
    {
        if (internalConverter is null) return;

        var genericArguments = internalConverter.GetType().BaseType!.GetGenericArguments();
        var convertTo = genericArguments[1];

        if (convertTo != typeof(string))
            throw new InvalidOperationException("The internal value converter must convert your type to a string to be used as an intermediary converter.");
    }
}