using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.DataProtection.Extensions;

/// <summary>
/// Provides extensions for the <see cref="PropertyBuilder"/> type.
/// </summary>
public static class PropertyBuilderExtensions
{
    internal const string IsEncryptedAnnotationName = "EntityFrameworkCore.DataProtection.IsEncrypted";
    internal const string IsQueryableAnnotationName = "EntityFrameworkCore.DataProtection.IsQueryable";

    /// <summary>
    /// Marks the property as encrypted.
    /// This will store the property as an encrypted string in the database.
    /// Supported types are <see cref="string"/> and <see cref="byte"/>[].
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isQueryable">
    /// when set to true, there will be generated a shadow property, that will
    /// hash the original value and store it alongside your property for querying.
    /// Use <see cref="QueryableExt.WherePdEquals{T}"/> to see how to query for protected data.
    /// </param>
    /// <typeparam name="TProperty">must be either string or byte[]</typeparam>
    /// <exception cref="NotImplementedException">if <see cref="TProperty"/> is neither string nor byte[]</exception>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<TProperty> IsEncrypted<TProperty>(this PropertyBuilder<TProperty> builder, bool isQueryable = false)
    {
        if (typeof(TProperty) != typeof(string) && typeof(TProperty) != typeof(byte[]))
            throw new InvalidOperationException("Only string and byte[] properties are supported for now. Please open an issue on https://github.com/ddjerqq/EntityFrameworkCore.DataProtection/issues to request a new feature");

        builder.HasAnnotation(IsEncryptedAnnotationName, true);
        builder.HasAnnotation(IsQueryableAnnotationName, isQueryable);
        return builder;
    }

    internal static (bool SupportsEncryption, bool SupportsQuerying) GetEncryptionStatus(this IMutableProperty property)
    {
        var encryptAttribute = property.PropertyInfo?.GetCustomAttribute<EncryptAttribute>();

        var supportsEncryption = encryptAttribute is not null || property.FindAnnotation(IsEncryptedAnnotationName)?.Value is true;
        var supportsQuerying = encryptAttribute?.IsQueryable is true || property.FindAnnotation(IsQueryableAnnotationName)?.Value is true;

        return (supportsEncryption, supportsQuerying);
    }
}