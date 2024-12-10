namespace EntityFrameworkCore.DataProtection.Test.Data;

internal sealed class User(Guid id, string name, string socialSecurityNumber, byte[] idPicture)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    [Encrypt(true)]
    public string SocialSecurityNumber { get; set; } = socialSecurityNumber;

    [Encrypt]
    public byte[] IdPicture { get; set; } = idPicture;
}