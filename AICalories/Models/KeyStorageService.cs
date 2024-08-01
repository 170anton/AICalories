using System;
using System.Text.Json;

namespace AICalories.Models
{
    public class KeyStorageService
    {
        private readonly EncryptionHelper _encryptionHelper;
        private readonly string _encryptedKeysFilePath;

        public KeyStorageService(string encryptionKey)
        {
            _encryptionHelper = new EncryptionHelper(encryptionKey);
            _encryptedKeysFilePath = GetAppDataPath();
        }

        private string GetAppDataPath()
        {
            string folderPath = FileSystem.AppDataDirectory;
            string filePath = Path.Combine(folderPath, "encryptedKeys.txt");
            return filePath;
        }

        public void StoreKeys(string awsAccessKeyId, string awsSecretAccessKey, string openAiApiKey)
        {
            var keys = new ApiKeys
            {
                AWSAccessKeyId = awsAccessKeyId,
                AWSSecretAccessKey = awsSecretAccessKey,
                OpenAIAPIKey = openAiApiKey
            };

            var jsonKeys = JsonSerializer.Serialize(keys);
            var encryptedKeys = _encryptionHelper.Encrypt(jsonKeys);

            File.WriteAllText(_encryptedKeysFilePath, encryptedKeys);
        }

        public ApiKeys RetrieveKeys()
        {
            var encryptedKeys = File.ReadAllText(_encryptedKeysFilePath);
            var decryptedKeys = _encryptionHelper.Decrypt(encryptedKeys);

            var keys = JsonSerializer.Deserialize<ApiKeys>(decryptedKeys);
            return keys;
        }
    }
}

