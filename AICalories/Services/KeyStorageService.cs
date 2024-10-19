using System;
using System.Text.Json;

namespace AICalories.Models
{
    public class KeyStorageService
    {
        private readonly EncryptionHelper _encryptionHelper;
        private readonly string _encryptedKeysFilePath;
        private const string resourceFileName = "AICalories.Resources.encryptedKeys.txt";


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

        public void StoreKeys(string openAiApiKey, string openAiApiKeyReserved)
        {
            var keys = new ApiKeys
            {
                OpenAIAPIKey = openAiApiKey,
                OpenAIAPIKeyReserved = openAiApiKeyReserved
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

            if (keys == null)
            {
                return new ApiKeys();
            }

            return keys;
        }
    }
}

