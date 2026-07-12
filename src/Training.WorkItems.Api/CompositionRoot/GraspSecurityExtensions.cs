using Grasp.Core.Security;
using Grasp.Core.Security.Encryption;
using Grasp.Core.Security.Enums;
using Grasp.Core.Security.Hashing;
using Grasp.Core.Security.Interfaces;
using Grasp.Core.Security.Providers;

namespace Training.WorkItems.Api.CompositionRoot;

public static class GraspSecurityExtensions
{
    private const string BootstrapKey = "GraspSecurity:VaultInitialized";
    private static readonly TimeSpan CacheTimeOut = TimeSpan.FromMinutes(30);

    public static WebApplicationBuilder AddGraspSecurity(this WebApplicationBuilder builder)
    {
        builder.InitializeGraspVault(builder.Configuration);
        builder.Services.AddGraspSecurityServices();
        return builder;
    }

    private static void InitializeGraspVault(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        if (builder.Properties.ContainsKey(BootstrapKey))
        {
            return;
        }

        builder.Services.InitializeVault(configuration);
        builder.Properties[BootstrapKey] = true;
    }

    private static void AddGraspSecurityServices(this IServiceCollection services)
    {
        services
            .AddSingleton(_ =>
                GraspSecure.CreateEncryptionService<string>(
                    EncryptionScheme.AesCbc256,
                    "GraspGlobalKEK",
                    "GraspGlobalDEK"))
            .AddSingleton(_ => GraspSecure.CreateHashProvider(HashingScheme.BCrypt12));
    }

    private static void InitializeVault(this IServiceCollection services, IConfiguration configuration)
    {
        var tenantId = GetRequiredSetting(configuration, "AppSettings:AKVTenantId");
        var clientId = GetRequiredSetting(configuration, "AppSettings:AKVClientId");
        var certificateName = GetRequiredSetting(configuration, "AppSettings:AKVCertificateName");
        var serverUri = new Uri(GetRequiredSetting(configuration, "AppSettings:AKVUri"));

        services.AddSingleton<ISecretsProvider>(
            new AzureSecretsVaultProvider(
                tenantId,
                clientId,
                certificateName,
                serverUri,
                true,
                CacheTimeOut));

        GraspSecure.Vault.RegisterProvider(
            "AKVKeys",
            new AzureKeyVaultProvider(
                tenantId,
                clientId,
                certificateName,
                serverUri,
                true,
                CacheTimeOut),
            VaultActions.Wrap | VaultActions.Unwrap | VaultActions.Get,
            KeyContexts.Kek);

        GraspSecure.Vault.RegisterProvider(
            "AKVSecrets",
            new AzureSecretsKeyVaultProxyProvider(
                tenantId,
                clientId,
                certificateName,
                serverUri,
                true,
                CacheTimeOut),
            VaultActions.Get,
            KeyContexts.Dek);

        GraspSecure.Vault.RegisterProvider(
            "AKVSecretsOnly",
            new AzureSecretsVaultProvider(
                tenantId,
                clientId,
                certificateName,
                serverUri,
                true,
                CacheTimeOut),
            VaultActions.Get);
    }

    private static string GetRequiredSetting(IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing required configuration value '{key}'.");
        }

        return value;
    }
}
