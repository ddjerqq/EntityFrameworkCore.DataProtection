using EntityFrameworkCore.DataProtection.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.DataProtection.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class DbContextOptionsBuilderExt
{
    /// <summary>
    /// Adds the <see cref="ShadowHashSynchronizerSaveChangesInterceptor"/> to the <see cref="DbContextOptionsBuilder"/>.
    /// This is crucial for synchronizing the shadow property hashes with the actual properties.
    /// </summary>
    public static DbContextOptionsBuilder AddDataProtectionInterceptors(this DbContextOptionsBuilder options) =>
        options.AddInterceptors(ShadowHashSynchronizerSaveChangesInterceptor.Instance);
}