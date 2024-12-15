using System.Text;
using EntityFrameworkCore.DataProtection.ValueConverters;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace Klean.EntityFrameworkCore.DataProtection.Test;

internal sealed class ValueConvertersTests
{
    [Test]
    public void Test_ValueConverters()
    {
        using var scope = Util.ServiceProvider.Value.CreateScope();
        var dataProtectorProvider = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>();
        var protector = dataProtectorProvider.CreateProtector("Klean.EntityFrameworkCore.DataProtection");

        var stringConverter = new StringDataProtectionValueConverter<string>(protector);
        var byteArrayConverter = new ByteArrayDataProtectionValueConverter<byte[]>(protector);

        const string payload = "Hello, World!";
        var encryptedString = stringConverter.ConvertToProvider(payload);
        Console.WriteLine(encryptedString);
        var decryptedString = stringConverter.ConvertFromProvider(encryptedString);
        decryptedString.Should().Be(payload);

        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var encryptedBytes = byteArrayConverter.ConvertToProvider(payloadBytes);
        Console.WriteLine(Convert.ToHexString((byte[])encryptedBytes!));
        var decryptedBytes = byteArrayConverter.ConvertFromProvider(encryptedBytes);
        decryptedBytes.Should().BeEquivalentTo(payloadBytes);
    }

    record Wrapper(string Value);

    class IntermediaryConverter() : ValueConverter<Wrapper, string>(
        to => to.Value,
        from => new Wrapper(from));

    [Test]
    public void Test_IntermediaryConverters()
    {
        using var scope = Util.ServiceProvider.Value.CreateScope();
        var dataProtectorProvider = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>();
        var protector = dataProtectorProvider.CreateProtector("Klean.EntityFrameworkCore.DataProtection");

        var intermediaryConverter = new IntermediaryConverter();
        var stringConverter = new StringDataProtectionValueConverter<Wrapper>(protector, intermediaryConverter);

        const string payload = "Hello, World!";
        var value = new Wrapper(payload);
        var encrypted = stringConverter.ConvertToProvider(value);
        Console.WriteLine(encrypted);
        var decryptedString = stringConverter.ConvertFromProvider(encrypted);
        decryptedString.Should().Be(value);
    }
}