using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.DataProtection.Extensions;

/// <summary>
/// Provides extensions for the <see cref="IServiceCollection"/> type.
/// </summary>
public static class ServiceCollectionExt
{
    /// <summary>
    /// Adds the data protection services to the service collection.
    /// This registers <see cref="Microsoft.Extensions.DependencyInjection.DataProtectionServiceCollectionExtensions"/> and returns the <see cref="IDataProtectionBuilder"/> instance.
    /// <see href="https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview">data protection configuration docs</see>
    /// </summary>
    /// <remarks>
    /// You need to configure Persistence with the returned <see cref="IDataProtectionBuilder"/> instance.
    /// see <see href="https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview#persistkeystofilesystem">persisting keys to file system</see>
    /// </remarks>
    /// <param name="applicationName">The application name to use with data protection</param>
    /// <returns>An instance of <see cref="IDataProtectionBuilder"/> used to configure the data protection services</returns>
    public static IDataProtectionBuilder AddDataProtectionServices(this IServiceCollection services, string applicationName)
    {
        return services.AddDataProtection()
            .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256,
                }
            )
            .SetApplicationName(applicationName);
    }
}