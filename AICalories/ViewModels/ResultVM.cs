using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Services;
using Newtonsoft.Json;

namespace AICalories.ViewModels
{
	public class ResultVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        private bool _isRefreshing;
        private bool _isLoading;
        private bool _isLabelVisible;
        private string _lastHistoryItemImage;
        private string _lastHistoryItemName;
        private string _lastHistoryItemCalories;
        private string _mealName;
        private string _weight;
        private string _calories;
        private string _proteins;
        private string _fats;
        private string _carbohydrates;
        private string _totalResultJSON;
        private ObservableCollection<IngredientItem> _ingredients;

        private ApiKeys _apiKeys;
        private readonly HttpClient _client = new HttpClient();
        private const string OpenAIAPIUrl = "https://api.openai.com/v1/chat/completions";

        private IImageInfo _imageInfo;

        #region Properties

        public bool IsLabelVisible
        {
            get => _isLabelVisible;
            set
            {
                _isLabelVisible = value;
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

        public ObservableCollection<IngredientItem> Ingredients
        {
            get => _ingredients;
            set
            {
                _ingredients = value;
                OnPropertyChanged();
            }
        }

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


        public string DishName
        {
            get => "Name: " + _mealName;
            set
            {
                if (_mealName != value)
                {

                    _mealName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Weight
        {
            get => "Weight: " + _weight + "g";
            set
            {
                if (_weight != value)
                {

                    _weight = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Calories
        {
            get => "Calories: " + _calories + "Cal";
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
            get => "Protein: " + _proteins + "g";
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
            get => "Fat: " + _fats + "g";
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
            get => "Carbs: " + _carbohydrates + "g";
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
            Ingredients = new ObservableCollection<IngredientItem>();

        }

        public async Task ProcessImage() //todo
        {
            try
            {
                await LoadSecrets();

                var imagePath = _imageInfo.ImagePath;
                if (imagePath != null)
                {
                    MealItem mealItem = await AnalyzeLocalImage(imagePath);

                    if (mealItem == null)
                    {
                        LoadAIResponse("Loading error");
                        return;
                    }
                    if (mealItem.IsMeal == false)
                    {
                        _alertService.ShowError("There is no food in this image.");
                        _navigationService.PopModalAsync();
                        return;
                    }
                    await AddItemToDB(imagePath, mealItem);

                    LoadAIResponse(mealItem);
                    await OnPageAppearingAsync();
                }
            }
            catch (JsonSerializationException)
            {
                _alertService.ShowError("Decoding error occurred");
                await _navigationService.NavigateToMainPageAsync();
            }
            catch (BadImageFormatException)
            {
                _alertService.ShowError("No connection to AI server");
                await _navigationService.NavigateToMainPageAsync();
            }
            catch (FileLoadException)
            {
                _alertService.ShowError("Error saving to database");
            }
            catch (Exception)
            {
                _alertService.ShowUnexpectedError();
                await _navigationService.NavigateToMainPageAsync();
            }
        }

        public void LoadAIResponse(MealItem mealItem)
        {
            IsRefreshing = false;
            DishName = mealItem.MealName;
            Weight = mealItem.Weight.ToString();
            Calories = ((IMealItem)mealItem).Calories.ToString();
            Proteins = mealItem.Proteins.ToString();
            Fats = ((IMealItem)mealItem).Fats.ToString();
            Carbohydrates = mealItem.Carbohydrates.ToString();
            TotalResultJSON = mealItem.TotalResultJSON;
        }

        public void LoadAIResponse(string response)
        {
            IsRefreshing = false;
            DishName = response;
        }


        public async Task OnPageAppearingAsync()
        {
            try
            {
                LastHistoryItemName = null;
                LastHistoryItemCalories = null;
                LastHistoryItemImage = null;

                await LoadLastMeal();

                //IsHistoryGridVisible = true;

            }
            catch (Exception)
            {
                //_alertService.ShowError("Loading error occurred");
            }
        }

        public async Task LoadLastMeal()
        {
            try
            {
                IsLabelVisible = false;
                IsLoading = true;
                await Task.Delay(500);
                var lastMeal = await App.HistoryItemRepository.GetLastMealItemAsync();
                IsLoading = false;

                if (lastMeal == null)
                {
                    IsLabelVisible = true;
                    return;
                }


                LastHistoryItemImage = lastMeal.ImagePath;
                LastHistoryItemName = lastMeal.MealName;
                LastHistoryItemCalories = ((IMealItem)lastMeal).Calories.ToString();

                await LoadLastMealIngredients(lastMeal.Id);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error LoadLastHistoryItem: {ex.Message}");
                throw;
            }
        }


        private async Task LoadLastMealIngredients(int lastMealId)
        {
            var lastMealIngredients = await App.IngredientItemRepository.GetIngredientsByMealIdAsync(lastMealId);

            Ingredients.Clear();
            foreach (var ingredient in lastMealIngredients)
            {
                Ingredients.Add(ingredient);
            }
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

        private async Task<MealItem> AnalyzeLocalImage(string imagePath)
        {
            try
            {
                string base64Image = ConvertToBase64(imagePath);

                HttpResponseMessage response = await SendRequestToApi(base64Image);

                string stringRawResult = await ExtractValidResponse(response);

                //dynamic mealItemJson = JsonConvert.DeserializeObject(stringRawResult);
                var mealItem = JsonConvert.DeserializeObject<MealItem>(stringRawResult);
                //var mealItem = new MealItem();
                //mealItem.IsMeal = mealItemJson.is_meal;
                //mealItem.MealName = mealItemJson.meal_name;
                //mealItem.Weight = mealItemJson.weight;
                //mealItem.Calories = mealItemJson.calories;
                //mealItem.Proteins = mealItemJson.proteins;
                //mealItem.Fats = mealItemJson.fats;
                //mealItem.Carbohydrates = mealItemJson.carbohydrates;
                mealItem.TotalResultJSON = stringRawResult;
                //mealItem.Ingredients = 

                return mealItem;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new JsonSerializationException();
            }

        }

        private static string ConvertToBase64(string imagePath)
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
            string base64Image = Convert.ToBase64String(imageArray);
            return base64Image;
        }

        private async Task<HttpResponseMessage> SendRequestToApi(string base64Image)
        {
            try
            {
                var requestData = RequestData.GetFirstPrompt(base64Image, _imageInfo.MealType, _imageInfo.UserInfo);
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(OpenAIAPIUrl, content);
                response.EnsureSuccessStatusCode();
                return response;

            }
            catch (Exception)
            {
                //DisplayAlertConfiguration.ShowError("This image is too big");
                throw new BadImageFormatException();
            }
        }

        private static async Task<string> ExtractValidResponse(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic rawResult = JsonConvert.DeserializeObject(responseString);
            string stringRawResult = rawResult.choices[0].message.tool_calls[0].function.arguments;
            return stringRawResult;
        }

        private async Task AddItemToDB(string imagePath, MealItem mealItem)
        {
            try
            {
                var dateTimeNow = DateTime.Now;

                mealItem.Date = dateTimeNow;
                mealItem.Time = dateTimeNow.ToString("HH:mm");
                mealItem.ImagePath = imagePath;

                await App.HistoryItemRepository.SaveMealItemAsync(mealItem);

                var mealItemId = (await App.HistoryItemRepository.GetLastMealItemAsync()).Id;
                foreach (var ingredient in mealItem.Ingredients)
                {
                    ingredient.MealItemId = mealItemId;
                    App.IngredientItemRepository.SaveIngredientAsync(ingredient);
                }

            }
            catch (Exception)
            {
                throw new FileLoadException();
            }

        }

        #endregion



        private async Task LoadSecrets()
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var encryptionKey = "eahuifuiwRHFwihHFIUwuia";
                var keyStorageService = new KeyStorageService(encryptionKey);



                _apiKeys = keyStorageService.RetrieveKeys();
                if (_apiKeys?.OpenAIAPIKey == null)
                {
                    if (_apiKeys?.OpenAIAPIKeyReserved == null)
                    {
                        throw new Exception();
                    }
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKeys?.OpenAIAPIKeyReserved}");
                    return;
                }
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKeys?.OpenAIAPIKey}");
                return;

            }
            catch (Exception)
            {
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

