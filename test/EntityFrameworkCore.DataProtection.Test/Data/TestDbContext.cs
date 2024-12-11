using EntityFrameworkCore.DataProtection.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.DataProtection.Test.Data;

internal sealed class TestDbContext(DbContextOptions<TestDbContext> options, IDataProtectionProvider dataProtectionProvider) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(TestDbContext).Assembly);
        base.OnModelCreating(builder);
        builder.UseDataProtection(dataProtectionProvider);
    }
}

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Email).IsEncrypted(true);
    }
}