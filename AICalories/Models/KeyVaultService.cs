using System;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace AICalories.Models
{
	public class KeyVaultService
	{
        private SecretClient _secretClient;
        private DeviceCodeCredential _deviceCodeCredential;

        public KeyVaultService()
        {
            // The DefaultAzureCredential will automatically find the appropriate authentication method.
            // It supports multiple authentication methods such as environment variables, managed identity, etc.
            //_secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());


            var keyVaultUrl = new Uri("https://aicalorieskeys.vault.azure.net/");
            var clientId = "76b95142-812a-4ea0-9db6-df2a0b820fd9"; 
            var tenantId = "1c9db038-466e-4692-8163-cf1f73dcc925";
            //var clientSecret = "W.j8Q~BsgFz~BS43B8DTexfVHcEcuytscU1l4byF";


            var options = new DeviceCodeCredentialOptions
            {
                ClientId = clientId,
                TenantId = tenantId,

                DeviceCodeCallback = (deviceCodeInfo, cancellationToken) =>
                {
                    Console.WriteLine(deviceCodeInfo.Message);
                    return Task.CompletedTask;
                }
            };

            _deviceCodeCredential = new DeviceCodeCredential(options);

            _secretClient = new SecretClient(keyVaultUrl, _deviceCodeCredential);

        }

        public async Task<string> GetSecretAsync(string secretName, string secretVersion)
        {
            using var cts = new CancellationTokenSource(5000); // Set the cancellation timeout to 5 seconds

            // Trigger the device code authentication flow explicitly
            //var tokenRequestContext = new TokenRequestContext(new[] { "https://vault.azure.net/.default" });
            //await _deviceCodeCredential.GetTokenAsync(tokenRequestContext, cts.Token);
            var secret = await _secretClient.GetSecretAsync(secretName, secretVersion, cts.Token);
            return secret.Value.Value;
        }

    }
}

