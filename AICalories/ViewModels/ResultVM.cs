﻿using System;
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
        private bool isRefreshing;
        private string mealName;
        private string calories;
        private string totalResultJSON;

        private ApiKeys _apiKeys;
        private readonly HttpClient _client = new HttpClient();
        private const string OpenAIAPIUrl = "https://api.openai.com/v1/chat/completions";

        private IImageInfo _imageInfo;

        #region Properties

        public string DishName
        {
            get => mealName;
            set
            {
                if (mealName != value)
                {

                    mealName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Calories
        {
            get => calories;
            set
            {
                if (calories != value)
                {
                    calories = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TotalResultJSON
        {
            get => totalResultJSON;
            set
            {
                if (totalResultJSON != value)
                {
                    totalResultJSON = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
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

            isRefreshing = true;

            LoadSecrets();
            if (_apiKeys.OpenAIAPIKey == null)
            {
                throw new ArgumentNullException();
            }
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKeys.OpenAIAPIKey}");

        }

        public async void ProcessImage() //todo
        {
            var image = _imageInfo.ImagePath;
            if (image != null)
            {
                var response = await ProcessImage(image);
                if (response == null)
                {
                    LoadAIResponse("Loading error");
                    return;
                }
                LoadAIResponse(response);
            }
        }

        public void LoadAIResponse(ResponseData response)
        {
            IsRefreshing = false;
            DishName = response.MealName;
            Calories = response.Calories.ToString();
            TotalResultJSON = response.TotalResultJSON;
        }

        public void LoadAIResponse(string response)
        {
            IsRefreshing = false;
            DishName = response;
        }



        #region Process Image

        public async Task<ResponseData> ProcessImage(string imagePath)
        {
            try
            {

                //var imagePath = image.Path;
                //imagePath = await ResizeImage(image.FullPath, 1000);

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
            responseData.MealName = result.meal_name;
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

