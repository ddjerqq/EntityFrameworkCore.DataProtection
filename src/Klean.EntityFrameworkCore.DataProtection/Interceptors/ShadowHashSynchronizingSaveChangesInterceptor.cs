using EntityFrameworkCore.DataProtection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.DataProtection.Interceptors;

/// <summary>
/// Interceptor to synchronize shadow hash properties with their original values.
/// </summary>
internal sealed class ShadowHashSynchronizerSaveChangesInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static readonly ShadowHashSynchronizerSaveChangesInterceptor Instance = new();

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        UpdateEntities(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, ct);
    }

    private static void UpdateEntities(DbContext? context)
    {
        if (context is null)
            return;

        var entries =
            from entry in context.ChangeTracker.Entries()
            where entry.State is EntityState.Added or EntityState.Modified
            let entity = entry.Entity
            let entityType = context.Model.FindEntityType(entity.GetType())
            where entityType is not null
            select (entry, entity, entityType);

        foreach (var (entry, entity, entityType) in entries)
            SynchronizeShadowHash(entityType, entity, entry);
    }

    private static void SynchronizeShadowHash(IEntityType entityType, object entity, EntityEntry entry)
    {
        var properties =
            from prop in entityType.GetProperties()
            let status = prop.GetEncryptionMetadata()
            where status is { SupportsEncryption: true, SupportsQuerying: true }
            select prop;

        foreach (var property in properties)
        {
            var originalValue = property.PropertyInfo?.GetValue(entity)?.ToString() ?? string.Empty;
            var shadowPropertyName = $"{property.Name}ShadowHash";
            var shadowProperty = entityType.FindProperty(shadowPropertyName);

            if (!string.IsNullOrWhiteSpace(originalValue) && shadowProperty is not null)
                entry.Property(shadowPropertyName).CurrentValue = originalValue.HmacSha256Hash();
        }
    }
}