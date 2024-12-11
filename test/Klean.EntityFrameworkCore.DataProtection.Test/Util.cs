using EntityFrameworkCore.DataProtection.Extensions;
using Klean.EntityFrameworkCore.DataProtection.Test.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Klean.EntityFrameworkCore.DataProtection.Test;

internal static class Util
{
    internal const string InMemoryDatabaseConnectionString = "Filename=:memory:";

    public static readonly Lazy<IServiceProvider> ServiceProvider = new(() =>
    {
        var services = new ServiceCollection();

        var connection = new SqliteConnection(InMemoryDatabaseConnectionString);
        connection.Open();

        services.AddDataProtectionServices("test");
        services.AddDbContext<TestDbContext>(opt => opt
            .AddDataProtectionInterceptors()
            .UseSqlite(connection));

        return services.BuildServiceProvider();
    });

    internal static TestDbContext CreateDbContext()
    {
        var scope = ServiceProvider.Value.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}