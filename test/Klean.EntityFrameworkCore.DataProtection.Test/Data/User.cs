using System.Security.Cryptography;
using EntityFrameworkCore.DataProtection;

namespace Klean.EntityFrameworkCore.DataProtection.Test.Data;

internal sealed class User
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    [Encrypt]
    public required string SocialSecurityNumber { get; init; }

    /// <summary>
    /// marked encrypted in the <see cref="UserConfiguration"/> class
    /// </summary>
    public required string Email { get; init; }

    [Encrypt(IsQueryable = false, IsUnique = false)]
    public required byte[] IdPicture { get; init; }

    public static User CreateRandom()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = RandomNumberGenerator.GetHexString(5),
            SocialSecurityNumber = RandomNumberGenerator.GetHexString(11),
            Email = RandomNumberGenerator.GetHexString(10),
            IdPicture = RandomNumberGenerator.GetBytes(256),
        };
    }
}