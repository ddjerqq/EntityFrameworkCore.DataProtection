using EntityFrameworkCore.DataProtection.Extensions;
using EntityFrameworkCore.DataProtection.Test.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.DataProtection.Test;

internal static class Util
{
    private const string InMemoryDatabaseConnectionString = "DataSource=:memory:";

    public static readonly Lazy<IServiceProvider> ServiceProvider = new(() =>
    {
        var services = new ServiceCollection();

        services.AddDbContext<TestDbContext>(opt => opt.UseSqlite(InMemoryDatabaseConnectionString));
        services.AddDataProtectionServices("test");

        return services.BuildServiceProvider();
    });

    internal static TestDbContext CreateDbContext() => ServiceProvider.Value.GetRequiredService<TestDbContext>();
}