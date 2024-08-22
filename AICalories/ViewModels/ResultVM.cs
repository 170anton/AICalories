using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Services;
using AndroidX.Lifecycle;
using Newtonsoft.Json;

namespace AICalories.ViewModels
{
	public class ResultVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        private bool _isRefreshing;
        private string _mealName;
        private string _calories;
        private string _proteins;
        private string _fats;
        private string _carbohydrates;
        private string _totalResultJSON;

        private ApiKeys _apiKeys;
        private readonly HttpClient _client = new HttpClient();
        private const string OpenAIAPIUrl = "https://api.openai.com/v1/chat/completions";

        private IImageInfo _imageInfo;

        #region Properties

        public string DishName
        {
            get => _mealName;
            set
            {
                if (_mealName != value)
                {

                    _mealName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Calories
        {
            get => _calories;
            set
            {
                if (_calories != value)
                {
                    _calories = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Proteins
        {
            get => _proteins;
            set
            {
                if (_proteins != value)
                {
                    _proteins = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Fats
        {
            get => _fats;
            set
            {
                if (_fats != value)
                {
                    _fats = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Carbohydrates
        {
            get => _carbohydrates;
            set
            {
                if (_carbohydrates != value)
                {
                    _carbohydrates = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TotalResultJSON
        {
            get => _totalResultJSON;
            set
            {
                if (_totalResultJSON != value)
                {
                    _totalResultJSON = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public ResultVM(IViewModelService viewModelService, IImageInfo imageInfo,
            INavigationService navigationService, IAlertService alertService)
        {
            _viewModelService = viewModelService;
            _viewModelService.ResultVM = this;
            _navigationService = navigationService;
            _alertService = alertService;
            _imageInfo = imageInfo;

            _isRefreshing = true;

            LoadSecrets();
            if (_apiKeys.OpenAIAPIKey == null)
            {
                throw new ArgumentNullException();
            }
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKeys.OpenAIAPIKey}");

        }

        public async void ProcessImage() //todo
        {
            var imagePath = _imageInfo.ImagePath;
            if (imagePath != null)
            {
                //var response = await ProcessImage(imagePath);

                ResponseData response = await AnalyzeLocalImage(imagePath);
                //ResponseData resultTwo = await AnalyzeLocalImage(imagePath);
                //ResponseData resultFinal = new ResponseData()
                //{
                //    DishName = resultTwo.DishName,
                //    Calories = (resultOne.Calories + resultTwo.Calories) / 2
                //};

                if (response == null)
                {
                    LoadAIResponse("Loading error");
                    return;
                }
                if (response.IsMeal == false)
                {
                    _alertService.ShowError("There is no food in this image.");
                    _navigationService.PopModalAsync();
                    return;
                }
                await AddItemToDB(imagePath, response);

                LoadAIResponse(response);
            }
        }

        public void LoadAIResponse(ResponseData response)
        {
            IsRefreshing = false;
            DishName = response.MealName;
            Calories = response.Calories.ToString();
            Proteins = response.Proteins.ToString();
            Fats = response.Fats.ToString();
            Carbohydrates = response.Carbohydrates.ToString();
            TotalResultJSON = response.TotalResultJSON;
        }

        public void LoadAIResponse(string response)
        {
            IsRefreshing = false;
            DishName = response;
        }



        #region Process Image

        //public async Task<ResponseData> ProcessImage(string imagePath)
        //{
        //    try
        //    {

        //        //var imagePath = image.Path;
        //        //imagePath = await ResizeImage(image.FullPath, 1000);

        //        ResponseData resultOne = await AnalyzeLocalImage(imagePath);
        //        //ResponseData resultTwo = await AnalyzeLocalImage(imagePath);
        //        //ResponseData resultFinal = new ResponseData()
        //        //{
        //        //    DishName = resultTwo.DishName,
        //        //    Calories = (resultOne.Calories + resultTwo.Calories) / 2
        //        //};
        //        await AddItemToDB(imagePath, resultOne);
        //        return resultOne;
        //    }
        //    catch (Exception)
        //    {
        //        //RequestShowAlert("No connection to OpenAI");
        //        throw;
        //    }
        //}

        private async Task<ResponseData> AnalyzeLocalImage(string imagePath)
        {
            string base64Image = ConvertToBase64(imagePath);

            HttpResponseMessage response = await SendRequestToApi(base64Image);

            string stringRawResult = await ExtractValidResponse(response);

            dynamic result = JsonConvert.DeserializeObject(stringRawResult);

            var responseData = new ResponseData();
            responseData.IsMeal = result.is_meal;
            responseData.MealName = result.meal_name;
            responseData.Calories = result.calories;
            responseData.Proteins = result.proteins;
            responseData.Fats = result.fats;
            responseData.Carbohydrates = result.carbohydrates;
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
            var requestData = RequestData.GetFirstPrompt(base64Image, _imageInfo.MealType);
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

        private async Task AddItemToDB(string imagePath, ResponseData responseData)
        {
            try
            {
                var dateTimeNow = DateTime.Now;
                var newItem = new HistoryItem
                {
                    Name = responseData.MealName,
                    Date = dateTimeNow,
                    Time = dateTimeNow.ToString("HH:mm"),
                    ImagePath = imagePath,
                    Calories = responseData.Calories.ToString(),
                    CaloriesInt = responseData.Calories
                };
                await App.HistoryDatabase.SaveItemAsync(newItem);


            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Sad");
            }

        }

        #endregion



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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

