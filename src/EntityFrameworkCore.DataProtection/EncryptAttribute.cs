namespace EntityFrameworkCore.DataProtection;

/// <summary>
/// Marks a property as encrypted.
/// </summary>
/// <remarks>
/// Because of how data protection in this library is implemented, if you want your protected property to be queryable, you must ensure that:
/// <para></para>
/// A) The property is a string or byte[].
/// <para></para>
/// B) The values of this property are unique (email addresses, full names, so on).
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EncryptAttribute : Attribute
{
    /// <summary>
    /// Gets a boolean value indicating if this property can be queried from the database.
    /// Because of how data protection in this library is implemented, if you want your protected property to be queryable, you must ensure that:
    /// <para></para>
    /// A) The property is a string or byte[].
    /// <para></para>
    /// B) The values of this property are unique (email addresses, full names, so on).
    /// </summary>
    public bool IsQueryable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptAttribute"/> class.
    /// indicates if this property can be queried from the database.
    /// Because of how data protection in this library is implemented, if you want your protected property to be queryable, you must ensure that:
    /// <para></para>
    /// A) The property is a string or byte[].
    /// <para></para>
    /// B) The values of this property are unique (email addresses, full names, so on).
    /// </summary>
    public EncryptAttribute(bool isQueryable = false) => IsQueryable = isQueryable;
}