using EntityFrameworkCore.DataProtection.Extensions;
using FluentAssertions;
using Klean.EntityFrameworkCore.DataProtection.Test.Data;
using Microsoft.EntityFrameworkCore;

namespace Klean.EntityFrameworkCore.DataProtection.Test;

internal sealed class DbContextTests
{
    [Test]
    public void Test_CreationWorks()
    {
        // arrange
        var user = User.CreateRandom();

        using (var dbContext = Util.CreateDbContext())
        {
            // act
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        using (var dbContext = Util.CreateDbContext())
        {
            // act
            var userFromDb = dbContext.Users
                .WherePdEquals(nameof(User.SocialSecurityNumber), user.SocialSecurityNumber)
                .First();

            // assert
            Assert.Multiple(() =>
            {
                userFromDb.Should().NotBeNull();
                userFromDb.Should().BeEquivalentTo(user);
                userFromDb.SocialSecurityNumber.Should().BeEquivalentTo(user.SocialSecurityNumber);
                userFromDb.IdPicture.Should().BeEquivalentTo(user.IdPicture);
            });
        }
    }

    [Test]
    public async Task Test_CreationWorksAsync()
    {
        // arrange
        var user = User.CreateRandom();

        await using (var dbContext = Util.CreateDbContext())
        {
            // act
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = Util.CreateDbContext())
        {
            // act
            var userFromDb = await dbContext.Users
                .WherePdEquals(nameof(User.SocialSecurityNumber), user.SocialSecurityNumber)
                .FirstAsync();

            // assert
            Assert.Multiple(() =>
            {
                userFromDb.Should().NotBeNull();
                userFromDb.Should().BeEquivalentTo(user);
                userFromDb.SocialSecurityNumber.Should().BeEquivalentTo(user.SocialSecurityNumber);
                userFromDb.IdPicture.Should().BeEquivalentTo(user.IdPicture);
            });
        }
    }
}