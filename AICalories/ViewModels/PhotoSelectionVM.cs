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
using Newtonsoft.Json.Linq;

namespace AICalories.ViewModels;

public class PhotoSelectionVM
{
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

    public async Task<ResponseData> ProcessImage(FileResult image)
    {
        try
        {

            var imagePath = image.FullPath;
            imagePath = await ResizeImage(image.FullPath, 500);
            //var stream = await image.OpenReadAsync();
            //capturedImage.Source = ImageSource.FromStream(() => stream);
            //string res = await ConvertImageToBase64(imagePath);
            //var imageUrl = await UploadImageToS3(image);
            //var result = await AnalyzeUrlImage(imageUrl);

            var result = await AnalyzeLocalImage(imagePath);
            //AddItemToDB(imagePath, result.Substring(0,8));

            return result;
        }
        catch (Exception)
        {
            //RequestShowAlert("No connection to OpenAI");
            throw;
        }
    }

    private async Task<ResponseData> AnalyzeLocalImage(string imagePath)
    {
        byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
        string base64Image = Convert.ToBase64String(imageArray);

        var requestData = RequestData.GetFirstPrompt(base64Image);
        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(OpenAIAPIUrl, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        dynamic rawResult = JsonConvert.DeserializeObject(responseString);

        string stringRawResult = rawResult.choices[0].message.tool_calls[0].function.arguments;

        dynamic result = JsonConvert.DeserializeObject(stringRawResult);

        //return rawResult.choices[0].message.content;
        //return jsonResponse.ToString(Formatting.Indented);
        var responseData = new ResponseData();
        responseData.DishName = result.dish_name;
        responseData.Calories = result.calories;

        return responseData;
        
    }

    private async Task<string> UploadImageToS3(FileResult image)
    {
        try
        {
            var imageStream = await image.OpenReadAsync();
            var fileName = image.FileName;

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

    private async Task<string> AnalyzeUrlImage(string imageUrl)
    {
        try
        {
            //byte[] imageArray = System.IO.File.ReadAllBytes(_photoPath);

            //string base64Image = Convert.ToBase64String(imageArray);

            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAIAPIKey}");

            var requestData = RequestData.GetSecondPrompt(imageUrl);

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(OpenAIAPIUrl, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);
            //var responseObject = JObject.Parse(responseString);
            //int result = responseObject["result"]?.Value<int>() ?? 0;
            //return result.ToString();
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
        try
        {
            var encryptionKey = "eahuifuiwRHFwihHFIUwuia";

            var keyStorageService = new KeyStorageService(encryptionKey);

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

    private async Task<string> ResizeImage(string imagePath, int maxRes)
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