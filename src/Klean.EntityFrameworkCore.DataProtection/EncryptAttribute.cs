namespace EntityFrameworkCore.DataProtection;

/// <summary>
/// Marks a property as encrypted.
/// Optionally choose if you want your property to be queryable or not.
/// Optionally choose if you want your properties to have a Unique Index or not.
/// </summary>
/// <remarks>
/// Because of how data protection in this library is implemented, if you want your protected property to be queryable, you must ensure that the property is a string or byte[].
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EncryptAttribute : Attribute
{
    /// <summary>
    /// Gets a boolean value indicating if this property can be queried from the database.
    /// Because of how data protection in this library is implemented, if you want your protected property to be queryable, you must ensure that the property is a string or byte[].
    /// </summary>
    internal bool IsQueryable { get; init; }

    /// <summary>
    /// Gets a boolean value indicating if this property should have a unique index.
    /// </summary>
    internal bool IsUnique { get; init; }

    /// <summary>
    /// Marks a property as encrypted.
    /// Optionally choose if you want your property to be queryable or not.
    /// Optionally choose if you want your properties to have a Unique Index or not.
    /// </summary>
    /// <remarks>
    /// Because of how data protection in this library is implemented, if you want your protected property to be queryable, you must ensure that the property is a string or byte[].
    /// Please note, you must set `EFCORE_DATA_PROTECTION__HASHING_SALT` in the environment to be able to query for data.
    /// </remarks>
    public EncryptAttribute(bool isQueryable, bool isUnique)
    {
        IsQueryable = isQueryable;
        IsUnique = isUnique;
    }
}