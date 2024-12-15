namespace Klean.EntityFrameworkCore.DataProtection.Test.Data;

public sealed record AddressData
{
    public string Country { get; init; }
    public string ZipCode { get; init; }

    public static AddressData Parse(string str) => str.Split('-') switch
    {
        [var country, var zipCode] => new AddressData { Country = country, ZipCode = zipCode },
        _ => throw new FormatException("Invalid format"),
    };

    public override string ToString() => $"{Country}-{ZipCode}";
}