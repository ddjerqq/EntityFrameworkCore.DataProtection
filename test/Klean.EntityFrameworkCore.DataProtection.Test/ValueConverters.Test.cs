using System.Text;
using EntityFrameworkCore.DataProtection.ValueConverters;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
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

        var stringConverter = new StringDataProtectionValueConverter(protector);
        var byteArrayConverter = new ByteArrayDataProtectionValueConverter(protector);

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
}