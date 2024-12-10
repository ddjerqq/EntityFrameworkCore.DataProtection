# EntityFrameworkCore.DataProtection

[![.NET](https://github.com/ddjerqq/EntityFrameworkCore.DataProtection/actions/workflows/build.yml/badge.svg)](https://github.com/ddjerqq/EntityFrameworkCore.DataProtection/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/EntityFrameworkCore.DataProtection.svg)](https://www.nuget.org/packages/EntityFrameworkCore.DataProtection)
[![Nuget Downloads](https://img.shields.io/nuget/dt/EntityFrameworkCore.DataProtection)](https://www.nuget.org/packages/EntityFrameworkCore.DataProtection)

`EntityFrameworkCore.DataProtection` is a [Microsoft Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) extension which
adds support for data protection and querying for encrypted properties for your entities.

## What problem does this library solve?

When you need to store sensitive data in your database, you may want to encrypt it to protect it from unauthorized access, however, when you
encrypt data, it becomes impossible to query it by EF-core, which is not really convenient if you want to encrypt, for example, email addresses, or SSNs
AND then filter entities by them.
This library (optionally) hashes the sensitive data and stores their sha256 hashes in a shadow property alongside the encrypted data. 
This allows you to query the encrypted data without decrypting it first. using `QueryableExt.WherePdEquals`

## Disclaimer

This project is maintained by [one (tenx) developer](https://github.com/ddjerqq) and is not affiliated with Microsoft.

I made this library to solve my own problems with EFCore. I needed to store a bunch of protected personal data encrypted, among these properties were personal IDs, Emails, SocialSecurityNumbers and so on.
As you know, you cannot query encrypted data with EFCore, and I wanted a simple yet boilerplate-free solution. Thus, I made this library.

**What this library allows you to do, is to encrypt your properties and query them without decrypting them first. It does so by hashing the encrypted data and storing the hash in a shadow property alongside the encrypted data.**

I **do not** take responsibility for any damage done in production environments and lose of your encryption key or corruption of your data.

Keeping your encryption keys secure is your responsibility. If you lose your encryption key, **you will lose your data.**

## Currently supported property types

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
        // then ApplyConfigurationsFromAssembly must come before base.OnModelCreating
        builder.ApplyConfigurationsFromAssembly(typeof(YourDbContext).Assembly);
        
        base.OnModelCreating(builder);
        
        //      UseDataProtection MUST come after base.OnModelCreating
        builder.UseDataProtection(dataProtectionProvider);
        
        // anything else you want to do with the builder must come after the call to UseDataProtection, unless you know exactly what you are doing
        builder.SnakeCaseRename();
    }
}
```

> [!WARNING]
> The call to `builder.UseDataProtection` **MUST** come after the call to `base.OnModelCreating` in your `DbContext` class
> and before any other configuration you might have.

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
> information on **how to configure the data protection** services, and how to store your encryption keys securely.

### Marking your properties as encrypted

There are three ways you can mark your properties as encrypted:

Using the EncryptedAttribute:
```csharp
class User
{
  [Encrypt(isQueryable: true)]
  public string SocialSecurityNumber { get; set; }

  [Encrypt]
  public byte[] IdPicture { get; set; }
}
```

Using the FluentApi (in your `DbContext.OnModelCreating` method):
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

Creating a custom `EntityTypeConfiguration` (Recommended for DDD):
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

> [!WARNING]
> The `QueryableExt.WherePdEquals` method is only available for properties that are marked as Queryable using the `[Encrypt(isQueryable: true)]` attribute or the
> `IsEncrypted(isQueryable: true)` method.

> [!TIP]
> The `WherePdEquals` extension method generates an expression like this one under the hood:<br/>
> `Where(e => EF.Property<string>(e, $"{propertyName}ShadowHash") == value.Sha256Hash())`

### Profit!

## Thank you for using this library!

ddjerqq <3