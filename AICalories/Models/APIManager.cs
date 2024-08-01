using System;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace AICalories.Models
{
	public class APIManager
	{
        private readonly IAmazonSecretsManager _aPIManagerClient;
        private string _secretName;

        public APIManager()
        {
            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            _secretName = "obj/Properties/OAIAWS";

            _aPIManagerClient = new AmazonSecretsManagerClient(
                                "AKIAQEIP3KP43FCWEAXH",
                                "xfqqNQ1flzpTpmAa8l2iZCm8CQR02TXkuOV++Dsy",
                                RegionEndpoint.EUNorth1);

        }

        public async Task<Dictionary<string, string>> GetSecretsAsync()
        {
            var cts = new CancellationTokenSource(2000);
            try
            {
                var request = new GetSecretValueRequest
                {
                    SecretId = _secretName,
                    //VersionStage = "AWSCURRENT",
                };

                var response = await _aPIManagerClient.GetSecretValueAsync(request);


                var originalSecrets = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);
                var swappedSecrets = originalSecrets.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

                return swappedSecrets;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("The operation has timed out.");
                throw new TimeoutException("The operation has timed out.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving secrets: {ex.Message}");
                throw;
            }
            finally
            {
                cts.Dispose();
            }
        }
    }
}

