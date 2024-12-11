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
    public bool IsQueryable { get; init; } = true;

    /// <summary>
    /// Gets a boolean value indicating if this property should have a unique index.
    /// </summary>
    public bool IsUnique { get; init; } = true;
}