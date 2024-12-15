using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.DataProtection.Extensions;

/// <summary>
/// Provides extensions for the <see cref="PropertyBuilder"/> type.
/// </summary>
public static class PropertyBuilderExt
{
    private const string IsEncryptedAnnotationName = "Klean.EntityFrameworkCore.DataProtection.IsEncrypted";
    private const string IsQueryableAnnotationName = "Klean.EntityFrameworkCore.DataProtection.IsQueryable";
    private const string IsUniqueIndexAnnotationName = "Klean.EntityFrameworkCore.DataProtection.IsUniqueIndex";

    /// <summary>
    /// Marks the property as encrypted.
    /// This will store the property as an encrypted string in the database.
    /// Supported types are <see cref="string"/> and <see cref="byte"/>[].
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TProperty">must be either string or byte[]</typeparam>
    /// <exception cref="NotImplementedException">if TProperty is neither string nor byte[]</exception>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<TProperty> IsEncrypted<TProperty>(this PropertyBuilder<TProperty> builder)
    {
        builder.HasAnnotation(IsEncryptedAnnotationName, true);
        return builder;
    }

    /// <summary>
    /// Marks the property as encrypted AND queryable.
    ///
    /// There will be generated a shadow property, that will
    /// hash the original value and store it alongside your property for querying.
    /// Use <see cref="QueryableExt.WherePdEquals{T}"/> to see how to query for protected data.
    ///
    /// This will store the property as an encrypted string in the database.
    /// Supported types are <see cref="string"/> and <see cref="byte"/>[].
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isUnique">
    /// If true, a unique index will be created for the shadow property. A regular index otherwise.
    /// </param>
    /// <typeparam name="TProperty">must be either string or byte[]</typeparam>
    /// <exception cref="NotImplementedException">if TProperty is neither string nor byte[]</exception>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<TProperty> IsEncryptedQueryable<TProperty>(this PropertyBuilder<TProperty> builder, bool isUnique = true)
    {
        builder.HasAnnotation(IsEncryptedAnnotationName, true);
        builder.HasAnnotation(IsQueryableAnnotationName, true);
        builder.HasAnnotation(IsUniqueIndexAnnotationName, isUnique);

        return builder;
    }

    internal static (bool SupportsEncryption, bool SupportsQuerying, bool IsUniqueIndex) GetEncryptionMetadata(this IReadOnlyProperty property)
    {
        var encryptAttribute = property.PropertyInfo?.GetCustomAttribute<EncryptAttribute>();

        var supportsEncryption = encryptAttribute is not null || property.FindAnnotation(IsEncryptedAnnotationName)?.Value is true;
        var supportsQuerying = encryptAttribute?.IsQueryable is true || property.FindAnnotation(IsQueryableAnnotationName)?.Value is true;
        var isUniqueIndex = encryptAttribute?.IsUnique is true || property.FindAnnotation(IsUniqueIndexAnnotationName)?.Value is true;

        return (supportsEncryption, supportsQuerying, isUniqueIndex);
    }
}