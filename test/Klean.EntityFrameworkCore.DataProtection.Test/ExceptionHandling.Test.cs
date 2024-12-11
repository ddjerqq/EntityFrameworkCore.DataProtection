using System.Reflection;
using EntityFrameworkCore.DataProtection;
using EntityFrameworkCore.DataProtection.Extensions;
using Klean.EntityFrameworkCore.DataProtection.Test.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Klean.EntityFrameworkCore.DataProtection.Test;

internal sealed class ExceptionHandlingTests
{
    [Test]
    public void Test_Throws_WhenServicesAreNotRegistered()
    {
        var services = new ServiceCollection();

        var connection = new SqliteConnection(Util.InMemoryDatabaseConnectionString);
        connection.Open();

        // services.AddDataProtectionServices("test");
        services.AddDbContext<BadDbContext>(opt => opt
            .AddDataProtectionInterceptors()
            .UseSqlite(connection));

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        Assert.Throws<InvalidOperationException>(() =>
        {
            using var dbContext = scope.ServiceProvider.GetRequiredService<BadDbContext>();
        });
    }

    [Test]
    public void Test_Throws_WhenIncorrectTypeIsMarkedAsEncrypted()
    {
        var services = new ServiceCollection();

        var connection = new SqliteConnection(Util.InMemoryDatabaseConnectionString);
        connection.Open();

        services.AddDataProtectionServices("test");
        services.AddDbContext<BadDbContext>(opt => opt
            .AddDataProtectionInterceptors()
            .UseSqlite(connection));

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<BadDbContext>();
    }

    class BadDbContext(IDataProtectionProvider dataProtectionProvider) : DbContext
    {
        public DbSet<BadEntity> BadEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BadDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseDataProtection(dataProtectionProvider);
        }
    }

    class BadEntity
    {
        [Encrypt]
        public Guid Id { get; set; }
    }
}