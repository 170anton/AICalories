using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Models;
using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media.Abstractions;
using SkiaSharp;

namespace AICalories.ViewModels;

public class MainVM : INotifyPropertyChanged
{
    private string _lastHistoryItemImage;
    private string _lastHistoryItemName;
    private string _lastHistoryItemCalories;
    private bool _isLoading;
    private bool _isLabelVisible;

    private ApiKeys _apiKeys;
    private readonly IViewModelService _viewModelService;
    private readonly HttpClient _client = new HttpClient();
    private const string OpenAIAPIUrl = "https://api.openai.com/v1/chat/completions";

    //public delegate Task ShowDelegate(string response);
    //public ShowDelegate OnShowResponseRequested;
    //public ShowDelegate OnShowAlertRequested;

    //public bool HasRecievedSecrets { get; set; }
    public ContextVM ContextVM => _viewModelService.ContextVM;
    public AppSettingsVM AppSettingsVM => _viewModelService.AppSettingsVM;

    #region Properties

    public string LastHistoryItemImage
    {
        get => _lastHistoryItemImage;
        set
        {
            _lastHistoryItemImage = value;
            OnPropertyChanged();
        }
    }


    public string LastHistoryItemName
    {
        get => _lastHistoryItemName;
        set
        {
            _lastHistoryItemName = value;
            OnPropertyChanged();
        }
    }

    public string LastHistoryItemCalories
    {
        get => _lastHistoryItemCalories;
        set
        {
            _lastHistoryItemCalories = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }



    public bool IsLabelVisible
    {
        get => _isLabelVisible;
        set
        {
            _isLabelVisible = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region Constructor

    public MainVM(IViewModelService viewModelService)
    {
        _viewModelService = viewModelService;
        _viewModelService.MainVM = this;

        LoadSecrets();
        if (_apiKeys.OpenAIAPIKey == null)
        {
            throw new ArgumentNullException();
        }

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKeys.OpenAIAPIKey}");
        //_s3Client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, BucketRegion);
        //_aPIManager = new APIManager();
        //LoadLastHistoryItem();
    }
    #endregion

    #region Process Image

    public async Task<ResponseData> ProcessImage(MediaFile image)
    {
        try
        {

            var imagePath = image.Path;
            //imagePath = await ResizeImage(image.FullPath, 1000);

            ResponseData resultOne = await AnalyzeLocalImage(imagePath);
            //ResponseData resultTwo = await AnalyzeLocalImage(imagePath);
            //ResponseData resultFinal = new ResponseData()
            //{
            //    DishName = resultTwo.DishName,
            //    Calories = (resultOne.Calories + resultTwo.Calories) / 2
            //};
            await AddItemToDB(imagePath, resultOne);
            LoadLastHistoryItem();
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
        var response = await _client.PostAsync(OpenAIAPIUrl, content);
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
                Name = responseData.DishName,
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

    private async Task<string> ResizeImageSkia(string imagePath, int maxRes)
    {
        using (var inputStream = File.OpenRead(imagePath))
        using (var original = SKBitmap.Decode(inputStream))
        {
            // Calculate the scaling factors
            float scale = Math.Max((float)maxRes / original.Width, (float)maxRes / original.Height);

            // Calculate the new dimensions
            int scaledWidth = (int)(original.Width * scale);
            int scaledHeight = (int)(original.Height * scale);

            // Create a new bitmap with the desired size
            var resizedBitmap = new SKBitmap(maxRes, maxRes);

            using (var canvas = new SKCanvas(resizedBitmap))
            {
                canvas.Clear(SKColors.Transparent);

                // Define the source rectangle (original image)
                var sourceRect = SKRect.Create(0, 0, original.Width, original.Height);

                // Define the destination rectangle (scaled and centered)
                var destRect = SKRect.Create(0, 0, maxRes, maxRes);
                canvas.DrawBitmap(original, sourceRect, destRect, new SKPaint { FilterQuality = SKFilterQuality.High });
            }

            // Save the resized image to the same file path (overwrite the original file)
            using (var outputStream = File.OpenWrite(imagePath))
            {
                SKImage.FromBitmap(resizedBitmap).Encode(SKEncodedImageFormat.Png, 100).SaveTo(outputStream);
            }
        }

        return imagePath;
    }

    #endregion



    public async void LoadLastHistoryItem()
    {
        try
        {
            IsLabelVisible = false;
            IsLoading = true;
            await Task.Delay(1000);
            var lastItem = await App.Database.GetLastItemAsync();
            IsLoading = false;

            if (lastItem == null)
            {
                IsLabelVisible = true;
                return;
            }

            LastHistoryItemImage = lastItem.ImagePath;
            LastHistoryItemName = lastItem.Name;
            LastHistoryItemCalories = lastItem.Calories;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error LoadLastHistoryItem: {ex.Message}");
            throw;
        }
    }

    private void LoadSecrets()
    {
        try
        {
            var encryptionKey = "eahuifuiwRHFwihHFIUwuia";

            var keyStorageService = new KeyStorageService(encryptionKey);


            _apiKeys = keyStorageService.RetrieveKeys();


            //HasRecievedSecrets = true;
        } 
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading secrets: {ex.Message}");
            throw;
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

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}