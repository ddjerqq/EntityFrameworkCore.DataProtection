using System.Linq.Expressions;
using EntityFrameworkCore.DataProtection.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.DataProtection.Extensions;

/// <summary>
/// Provides extensions for the <see cref="IQueryable{T}"/> type.
/// </summary>
public static class QueryableExt
{
    /// <summary>
    /// Queries the entity by personal data. Please make sure your property is marked as encrypted (using <see cref="EncryptAttribute"/>) AND queryable (<see cref="EncryptAttribute.IsQueryable"/>)
    /// </summary>
    /// <remarks>
    /// You MUST call <see cref="DbContextOptionsBuilderExt.AddDataProtectionInterceptors"/> on the DbContext options to enable this feature.
    /// <code>
    /// services.AddDbContext&lt;YourDbContext&gt;(opt => opt
    ///   .AddDataProtectionInterceptors()
    ///   .UseWhateverYouHave());
    /// </code>
    /// <para></para>
    /// The property you are querying for MUST be marked with <see cref="EncryptAttribute"/> and <see cref="EncryptAttribute.IsQueryable"/> set to true.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// var foo = await DbContext.Users
    ///   .WherePdEquals(nameof(User.SocialSecurityNumber), "404-69-1337")
    ///   .SingleOrDefaultAsync();
    /// </code>
    /// <para></para>
    ///
    /// Generates an expression like this under the hood:
    /// <code>
    /// e => EF.Property&lt;string&gt;(e, $"{propertyName}ShadowHash") == value.Sha256Hash()
    /// </code>
    /// <para></para>
    ///
    /// User class for this example:
    /// <code>
    /// class User
    /// {
    ///   [Encrypt(IsQueryable = true)]
    ///   public string SocialSecurityNumber { get; set; }
    /// }
    /// </code>
    /// </example>
    /// <param name="query">The query to filter</param>
    /// <param name="propertyName">The name of the property. use nameof() expressions</param>
    /// <param name="value">The value to compare against</param>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <returns>Filtered query</returns>
    public static IQueryable<T> WherePdEquals<T>(this IQueryable<T> query, string propertyName, string value)
    {
        var shadowPropertyName = $"{propertyName}ShadowHash";

        // e => EF.Property<string>(e, shadowPropertyHash) == value.Sha256Hash()

        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(string) },
            parameter,
            Expression.Constant(shadowPropertyName));

        var comp = Expression.Equal(property, Expression.Constant(value.HmacSha256Hash()));
        var lambda = Expression.Lambda<Func<T, bool>>(comp, parameter);

        return query.Where(lambda);
    }
}