using System.Drawing;
using System.Text;
using System.Windows.Input;
using AICalories.Models;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json;

namespace AICalories.ViewModels;

public class PhotoSelectionVM
{
    private string _photoPath;

    private string OpenAIAPIKey;
    private string AwsAccessKeyId;
    private string AwsSecretAccessKey;

    private readonly HttpClient client = new HttpClient();
    private const string OpenAIAPIUrl = "https://api.openai.com/v1/chat/completions";
    private const string BucketName = "aic-images";
    private readonly RegionEndpoint BucketRegion = RegionEndpoint.EUNorth1;
    private IAmazonS3 _s3Client;
    //private readonly APIManager _aPIManager;

    //public delegate Task ShowDelegate(string response);
    //public ShowDelegate OnShowResponseRequested;
    //public ShowDelegate OnShowAlertRequested;

    public string PhotoPath { get => _photoPath; set => _photoPath = value; }
    //public bool HasRecievedSecrets { get; set; }

    public PhotoSelectionVM()
    {
        LoadSecrets();
        if (OpenAIAPIKey == null)
        {
            throw new ArgumentNullException();
        }

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAIAPIKey}");
        _s3Client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, BucketRegion);
        //_aPIManager = new APIManager();
    }

    public async Task<string> ProcessImage(FileResult image)
    {
        try
        {

            //_photoPath = image.FullPath;
            _photoPath = ResizeImage(image.FullPath, 2000);
            var stream = await image.OpenReadAsync();
            //capturedImage.Source = ImageSource.FromStream(() => stream);

            var imageUrl = await UploadImageToS3(stream, image.FileName);
            var result = await AnalyzeImageWithOpenAI(imageUrl);
            AddItemToDB(_photoPath, result.Substring(0,8));

            //await OnShowResponseRequested(result);
            return result;
        }
        catch (Exception)
        {
            //RequestShowAlert("No connection to OpenAI");
            throw;
        }
    }

    private async Task<string> UploadImageToS3(Stream imageStream, string fileName)
    {
        try
        {
            var uploadRequest = new PutObjectRequest
            {
                InputStream = imageStream,
                BucketName = BucketName,
                Key = fileName,
                ContentType = "image/jpeg"
            };

            var response = await _s3Client.PutObjectAsync(uploadRequest);
            return $"https://{BucketName}.s3.{BucketRegion.SystemName}.amazonaws.com/{fileName}";
        }
        catch (Exception)
        {
            throw;
        }
        
    }
    private async Task<string> DeleteImageFromS3(Stream imageStream, string fileName)
    {
        try
        {
            var uploadRequest = new PutObjectRequest
            {
                InputStream = imageStream,
                BucketName = BucketName,
                Key = fileName,
                ContentType = "image/jpeg"
            };

            var response = await _s3Client.PutObjectAsync(uploadRequest);
            return $"https://{BucketName}.s3.{BucketRegion.SystemName}.amazonaws.com/{fileName}";
        }
        catch (Exception)
        {
            throw;
        }

    }
    private async Task<string> AnalyzeImageWithOpenAI(string imageUrl)
    {
        try
        {
            //byte[] imageArray = System.IO.File.ReadAllBytes(_photoPath);

            //string base64Image = Convert.ToBase64String(imageArray);

            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAIAPIKey}");
            var requestData = new
            {
                max_tokens = 100,
                //temperature = 0.1,
                model = "gpt-4o",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = "How much calories is this dish?"
                            },
                            new
                            {
                                type = "image_url",
                                image_url = new
                                {
                                    url = imageUrl
                                }
                            }
                        }
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(OpenAIAPIUrl, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);

            return result.choices[0].message.content;
        }
        catch (HttpRequestException httpRequestException)
        {
            return $"Request error: {httpRequestException.Message}";
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }

    private void LoadSecrets()
    {
        //var keyVaultService = new KeyVaultService();

        try
        {
            //AwsAccessKeyId = "AKIAQEIP3KP43FCWEAXH";
            //AwsSecretAccessKey = "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";
            //    //AwsAccessKeyId = await keyVaultService.GetSecretAsync("AwsAccessKeyId");
            //    //AwsSecretAccessKey = await keyVaultService.GetSecretAsync("AwsSecretAccessKey");
            //OpenAIAPIKey = await keyVaultService.GetSecretAsync("OpenAIAPIKey", "1f734d43cdb646e19c215c4e7cb25ccf");
            //    //OpenAIAPIKey = await _secretClient.GetSecretAsync(secretName)

            //    //var secrets = await _aPIManager.GetSecretsAsync();

            //    //AwsAccessKeyId = secrets["AwsAccessKeyId"];
            //    //AwsSecretAccessKey = secrets["AwsSecretAccessKey"];
            //    //OpenAIAPIKey = secrets["OpenAIAPIKey"];

            var encryptionKey = "eahuifuiwRHFwihHFIUwuia";

            var keyStorageService = new KeyStorageService(encryptionKey);

            // Store keys securely

            // Retrieve and use keys
            var keys = keyStorageService.RetrieveKeys();

            Console.WriteLine($"AWS Access Key ID: {keys.AWSAccessKeyId}");
            Console.WriteLine($"AWS Secret Access Key: {keys.AWSSecretAccessKey}");
            Console.WriteLine($"OpenAI API Key: {keys.OpenAIAPIKey}");

            OpenAIAPIKey = keys.OpenAIAPIKey;
            AwsAccessKeyId = keys.AWSAccessKeyId;
            AwsSecretAccessKey = keys.AWSSecretAccessKey;

            //HasRecievedSecrets = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading secrets: {ex.Message}");
        }
    }
    private async void AddItemToDB(string? image, string? calories)
    {
        try
        {
            var dateTimeNow = DateTime.Now;
            var newItem = new HistoryItem
            {
                Date = dateTimeNow,
                Time = dateTimeNow.ToString("HH:mm"),
                ImagePath = image,
                Calories = calories
            };
            await App.Database.SaveItemAsync(newItem);

        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Sad");
        }

    }

    private string ResizeImage(string imagePath, int maxRes)
    {
        string tempFilePath = Path.GetTempFileName();

        try
        {
            using (var inputStream = File.OpenRead(imagePath))
            {
                var image = PlatformImage.FromStream(inputStream, ImageFormat.Jpeg);
                var resizedImage = image.Downsize(maxRes);

                using (var outputStream = File.Create(tempFilePath))
                {
                    resizedImage.Save(outputStream, ImageFormat.Jpeg);
                }
            }
            File.Copy(tempFilePath, imagePath, true);

            return imagePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resizing image: {ex.Message}");
            return null;
        }
        finally
        {
            // Clean up temporary file
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    //public void RequestShowResponse(string response)
    //{
    //    OnShowResponseRequested?.Invoke(response);
    //}

    //public void RequestShowAlert(string response)
    //{
    //    //HasRecievedSecrets = false;
    //    OnShowAlertRequested?.Invoke(response);
    //}
}