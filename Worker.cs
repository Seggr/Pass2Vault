using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pass2Vault
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private string secretName = Dns.GetHostName();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Inspecting secrets at: {time}", DateTimeOffset.Now);

                // <authenticate>
                var kvUri = "https://" + Environment.GetEnvironmentVariable("AZURE_VAULT_NAME") + ".vault.azure.net";
                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                // </authenticate>

                var secretValue = passworder.GetRandomAlphanumericString(48);
                KeyVaultSecret superSecret = client.GetSecret(secretName);

                if (superSecret.Properties.ExpiresOn.HasValue)
                {
                    _logger.LogDebug("Secret is valid and will expire");
                }
                else {
                    _logger.LogDebug("Secret is not set to expire!");
                }

                if (superSecret.Properties.ExpiresOn.GetValueOrDefault() > DateTimeOffset.Now)
                {
                    _logger.LogDebug($"Secret is still valid and Expires at: {superSecret.Properties.ExpiresOn.GetValueOrDefault()}");
                }

                if (superSecret.Properties.ExpiresOn.HasValue && superSecret.Properties.ExpiresOn.GetValueOrDefault() < DateTimeOffset.Now)
                {

                    // Create a new secret.
                    _logger.LogDebug("Creating new Secret");
                    KeyVaultSecret newSuperSecret = new KeyVaultSecret(secretName, secretValue);
                    newSuperSecret.Properties.ExpiresOn = DateTimeOffset.Now.AddHours(8);
                    client.SetSecret(newSuperSecret);

                    try
                    {
                        // Change the current password.
                        Netadpi32.ChangePassword("sysadmin",null,superSecret.Value,newSuperSecret.Value);
                    }
                    catch
                    {
                        // Disable the newly created secret.
                        _logger.LogInformation("FUUUUCCCCCK");
                    }

                    // Delete old Secret??????
                    // Maybe disable Secret?

                }

                await Task.Delay(7200000, stoppingToken);
            }
        }

    }
}
