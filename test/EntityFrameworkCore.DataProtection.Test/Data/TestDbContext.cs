using EntityFrameworkCore.DataProtection.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.DataProtection.Test.Data;

internal sealed class TestDbContext(DbContextOptions<TestDbContext> options, IDataProtectionProvider dataProtectionProvider) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseDataProtection(dataProtectionProvider);
    }
}