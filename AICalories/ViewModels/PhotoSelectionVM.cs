using System.Drawing;
using System.Text;
using System.Windows.Input;
using AICalories.DI;
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

    private readonly IViewModelService _viewModelService;
    private readonly HttpClient client = new HttpClient();
    private const string OpenAIAPIUrl = "https://api.openai.com/v1/chat/completions";
    //private const string BucketName = "aic-images";
    //private readonly RegionEndpoint BucketRegion = RegionEndpoint.EUNorth1;
    //private IAmazonS3 _s3Client;
    //private readonly APIManager _aPIManager;

    //public delegate Task ShowDelegate(string response);
    //public ShowDelegate OnShowResponseRequested;
    //public ShowDelegate OnShowAlertRequested;

    //public bool HasRecievedSecrets { get; set; }
    public ContextVM ContextVM => _viewModelService.ContextVM;
    public AppSettingsVM AppSettingsVM => _viewModelService.AppSettingsVM;


    #region Constructor

    public PhotoSelectionVM(IViewModelService viewModelService)
    {
        _viewModelService = viewModelService;
        _viewModelService.PhotoSelectionVM = this;

        LoadSecrets();
        if (OpenAIAPIKey == null)
        {
            throw new ArgumentNullException();
        }

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAIAPIKey}");
        //_s3Client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, BucketRegion);
        //_aPIManager = new APIManager();
    }
    #endregion

    #region Process Image

    public async Task<ResponseData> ProcessImage(FileResult image)
    {
        try
        {

            var imagePath = image.FullPath;
            imagePath = await ResizeImage(image.FullPath, 1000);

            ResponseData resultOne = await AnalyzeLocalImage(imagePath);
            //ResponseData resultTwo = await AnalyzeLocalImage(imagePath);
            //ResponseData resultFinal = new ResponseData()
            //{
            //    DishName = resultTwo.DishName,
            //    Calories = (resultOne.Calories + resultTwo.Calories) / 2
            //};
            await AddItemToDB(imagePath, resultOne);

            return resultOne;
        }
        catch (Exception)
        {
            //RequestShowAlert("No connection to OpenAI");
            throw;
        }
    }

    private async Task<ResponseData> AnalyzeLocalImage(string imagePath)
    {
        string base64Image = ConvertToBase64(imagePath);

        HttpResponseMessage response = await SendRequestToApi(base64Image);

        string stringRawResult = await ExtractValidResponse(response);

        dynamic result = JsonConvert.DeserializeObject(stringRawResult);

        var responseData = new ResponseData();
        responseData.DishName = result.dish_name;
        responseData.Calories = result.calories;
        responseData.TotalResultJSON = stringRawResult;

        return responseData;

    }

    private static string ConvertToBase64(string imagePath)
    {
        byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
        string base64Image = Convert.ToBase64String(imageArray);
        return base64Image;
    }

    private async Task<HttpResponseMessage> SendRequestToApi(string base64Image)
    {
        var requestData = RequestData.GetFirstPrompt(base64Image, ContextVM.SelectedOption);
        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(OpenAIAPIUrl, content);
        response.EnsureSuccessStatusCode();
        return response;
    }

    private static async Task<string> ExtractValidResponse(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic rawResult = JsonConvert.DeserializeObject(responseString);
        string stringRawResult = rawResult.choices[0].message.tool_calls[0].function.arguments;
        return stringRawResult;
    }

    private async Task AddItemToDB(string image, ResponseData responseData)
    {
        try
        {
            var dateTimeNow = DateTime.Now;
            var newItem = new HistoryItem
            {
                Date = dateTimeNow,
                Time = dateTimeNow.ToString("HH:mm"),
                ImagePath = image,
                Calories = responseData.Calories.ToString(),
                CaloriesInt = responseData.Calories
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

                if (maxRes >= image.Width && maxRes >= image.Height)
                {
                    return imagePath;
                }

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

    #endregion

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