# EntityFrameworkCore.DataProtection

[![.NET](https://github.com/ddjerqq/EntityFrameworkCore.DataProtection/actions/workflows/build.yml/badge.svg)](https://github.com/ddjerqq/EntityFrameworkCore.DataProtection/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/EntityFrameworkCore.DataProtection.svg)](https://www.nuget.org/packages/EntityFrameworkCore.DataProtection)
[![Nuget Downloads](https://img.shields.io/nuget/dt/EntityFrameworkCore.DataProtection)](https://www.nuget.org/packages/EntityFrameworkCore.DataProtection)

`EntityFrameworkCore.DataProtection` is a [Microsoft Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) extension which
adds support for data protection and querying for encrypted properties for your entities.

## What problem does this library solve?

When you need to store sensitive data in your database, you may want to encrypt it to protect it from unauthorized access. However, when you
encrypt data, it becomes unreadable by EF-core, which is not really convenient if you want to encrypt, for example, email addresses, or SSNs
AND then query them.
This library (optionally) hashes the sensitive data and stores their sha256 hashes in a shadow property alongside the encrypted data. This
allows you to query the encrypted data without decrypting it first. using `QueryableExt.WherePdEquals`

## Disclaimer

This project is maintained by <a href="https://github.com/ddjerqq">ddjerqq</a> and is not affiliated with Microsoft.

I made this library initially to solve my own problems with EFCore when I needed to encrypt personal IDs but also query them. I wanted a
simple yet boilerplate-free solution. Thus, I made this library.

I **do not** take responsibility for any damage done in production environments and lose of your encryption key or corruption of your data.

Keeping your encryption keys secure is your responsibility. If you lose your encryption key, **you will lose your data.**

## Supported property types

- string
- byte[]

## Getting started

### Installing the package

Install the package from [NuGet](https://www.nuget.org/) or from the `Package Manager Console` :

```powershell
PM> Install-Package EntityFrameworkCore.DataProtection
```

### Configuring Data Protection in your DbContext

`YourDbContext.cs`

```csharp
//                                                vv injected from DI
public class Your(DbContextOptions<Your> options, IDataProtectionProvider dataProtectionProvider) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // if you are using IEntityTypeConfiguration<T> in your project
        // then ApplyConfigurationsFromAssembly must come before base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(YourDbContext).Assembly);
        
        base.OnModelCreating(builder);
        
        // MUST COME AFTER `base.OnModelCreating(builder);`
        builder.UseDataProtection(dataProtectionProvider);
        
        // anything else you want to do with the builder must come after the call to UseDataProtection
        builder.SnakeCaseRename();
    }
}
```

> [!WARNING]
> The call to `builder.UseDataProtection` **MUST** come after the call to `base.OnModelCreating` in your `DbContext` class.
> And before any other configuration you might have.

### Registering the services

`Program.cs`

```csharp
builder.Services.AddDbContext<YourDbContext>(/* ... */);

var keyDirectory = new DirectoryInfo("keys");
builder.Services.AddDataProtectionServices()
    .PersistKeysToFileSystem(keyDirectory);
```

> [!TIP]
> See the [Microsoft documentation](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview) for more
> information on how to configure the data protection services, and how to store your encryption keys securely.

### Marking your properties as encrypted

There are three ways you can mark your properties as encrypted:

Using the EncryptedAttribute:
```csharp
class User
{
  public Guid Id { get; set; }

  public string Name { get; set; }
  
  [Encrypt(true)]
  public string SocialSecurityNumber { get; set; }

  [Encrypt]
  public byte[] IdPicture { get; set; }
}
```

Or using the FluentApi (in your `DbContext.OnModelCreating` method):
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.Entity<User>(entity =>
    {
        entity.Property(e => e.SocialSecurityNumber).IsEncrypted(true);
        entity.Property(e => e.IdPicture).IsEncrypted();
    });
}
```

Or creating a custom `EntityTypeConfiguration`:
```csharp
class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.SocialSecurityNumber).IsEncrypted(true);
        builder.Property(e => e.IdPicture).IsEncrypted();
    }
}
```

### Querying encrypted properties

You can query encrypted properties that are marked as Queryable using the `QueryableExt.WherePdEquals` extension method:

```csharp
var foo = await DbContext.Users
  .WherePdEquals(nameof(User.SocialSecurityNumber), "404-69-1337")
  .SingleOrDefaultAsync();
```

> [!TIP]
> Generates an expression like this under the hood:
> `Where(e => EF.Property<string>(e, $"{propertyName}ShadowHash") == value.Sha256Hash())`

> [!WARNING]
> The `QueryableExt.WherePdEquals` method is only available for properties that are marked as Queryable using the `[Encrypt(isQueryable: true)]` attribute or the
> `IsEncrypted(isQueryable: true)` method.

### Profit!

## Thank you for using this library!

ddjerqq <3