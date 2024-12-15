using EntityFrameworkCore.DataProtection.ValueConverters;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.DataProtection.Extensions;

/// <summary>
/// Provides extensions for the <see cref="ModelBuilder"/> type.
/// </summary>
public static class ModelBuilderExt
{
    /// <summary>
    /// Configures the data protection for the entities marked with <see cref="EncryptAttribute"/> or Fluently marked
    /// as Encrypted using <see cref="PropertyBuilderExt.IsEncrypted{TProperty}"/>.
    /// </summary>
    /// <remarks>
    /// You must call this method __after__ <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/>.
    /// You must call this method __after__ <see cref="DbContext.OnModelCreating"/>.
    /// You must call this method __before__ any custom configuration you have (for example global dbo renamers or soft deletions).
    /// </remarks>
    /// <example>
    /// <code>
    /// public class AppDbContext(IDataProtectionProvider dataProtectionProvider) : DbContext
    /// {
    ///   protected override void OnModelCreating(ModelBuilder builder)
    ///   {
    ///     builder.ApplyConfigurationsFromAssembly(/* ... */);
    ///     base.OnModelCreating(builder);
    ///     builder.UseSoftDeletion();
    ///     <para></para>
    ///     builder.UseDataProtection(dataProtectionProvider);
    ///     <para></para>
    ///     builder.SnakeCaseRename();
    ///   }
    /// }
    /// </code>
    /// </example>
    /// <param name="builder"></param>
    /// <param name="dataProtectionProvider">
    /// The <see cref="IDataProtectionProvider"/> instance to use for data protection.
    /// </param>
    /// <returns>The same <see cref="ModelBuilder"/> instance so that additional configuration calls can be chained.</returns>
    /// <exception cref="NotImplementedException">
    /// If there are properties decorated with the <see cref="EncryptAttribute"/> that are not string or byte[]
    /// </exception>
    public static ModelBuilder UseDataProtection(this ModelBuilder builder, IDataProtectionProvider dataProtectionProvider)
    {
        var protector = dataProtectionProvider.CreateProtector("Klean.EntityFrameworkCore.DataProtection");

        var properties = (
                from entityType in builder.Model.GetEntityTypes()
                from prop in entityType.GetProperties()
                let status = prop.GetEncryptionMetadata()
                where status.SupportsEncryption
                select (entityType, prop, status))
            .ToList();

        foreach (var (entityType, property, status) in properties)
        {
            var propertyType = property.PropertyInfo?.PropertyType;
            var internalConverter = property.GetValueConverter();

            var internalConverterConvertTo = internalConverter?.GetType().BaseType?.GetGenericArguments()[1];

            if (propertyType == typeof(string) || internalConverterConvertTo == typeof(string))
            {
                var converter = typeof(StringDataProtectionValueConverter<>).MakeGenericType(propertyType!);
                var instance = Activator.CreateInstance(converter, protector, internalConverter);
                property.SetValueConverter(instance as ValueConverter);
            }
            else if (propertyType == typeof(byte[]) || internalConverterConvertTo == typeof(byte[]))
            {
                var converter = typeof(ByteArrayDataProtectionValueConverter<>).MakeGenericType(propertyType!);
                var instance = Activator.CreateInstance(converter, protector, internalConverter);
                property.SetValueConverter(instance as ValueConverter);
            }
            else if (internalConverter is null)
                throw new InvalidOperationException(
                    "Only string and byte[] properties are supported for now. Alternatively use an intermediary converter (see esoteric readme section)" +
                    "Please open an issue on https://github.com/ddjerqq/Klean.EntityFrameworkCore.DataProtection/issues to request a new feature");

            if (status.SupportsQuerying)
                AddShadowProperty(entityType, property, status.IsUniqueIndex);
        }

        return builder;
    }

    private static void AddShadowProperty(IMutableEntityType entityType, IMutableProperty property, bool isUniqueIndex)
    {
        var originalPropertyName = property.Name;
        var shadowPropertyName = $"{originalPropertyName}ShadowHash";

        if (entityType.GetProperties().Any(p => p.Name == shadowPropertyName))
            return;

        var shadowProperty = entityType.AddProperty(shadowPropertyName, typeof(string));
        shadowProperty.IsShadowProperty();

        if (isUniqueIndex)
            shadowProperty.IsUniqueIndex();
        else
            shadowProperty.IsIndex();

        shadowProperty.IsNullable = property.IsNullable;
    }
}