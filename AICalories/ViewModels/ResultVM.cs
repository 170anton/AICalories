using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Services;
using Android.App;
using Android.Gms.Ads;
using Android.Gms.Ads.Interstitial;
using Android.Views;
using Newtonsoft.Json;

namespace AICalories.ViewModels
{
	public class ResultVM : INotifyPropertyChanged
    {
        private InterstitialAd _interstitialAd;
        private readonly IViewModelService _viewModelService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        private readonly string _adUnitId = "ca-app-pub-9280044316923474/7763621828";
        //"ca-app-pub-3940256099942544/1033173712"; - for testing
        //"ca-app-pub-9280044316923474/7763621828"; - actual

        //private bool _isRefreshing;
        private bool _isAdsVisible;
        private bool _isLoading;
        private bool _isLabelVisible;
        private bool _isHistoryGridVisible;
        private MealItem _lastHistoryItem;
        private string _lastHistoryItemImage;
        private string _lastHistoryItemName;
        private string _lastHistoryItemCalories;
        private string _lastHistoryItemProtein;
        private string _lastHistoryItemFat;
        private string _lastHistoryItemCarbs;
        private string _lastHistoryItemSugar;
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

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NewImageCommand { get; }
        public ICommand LoadAdCommand { get; }
        public ICommand ShowAdCommand { get; }

        #region Properties

        public MealItem LastHistoryItem
        {
            get => _lastHistoryItem;
            set
            {
                _lastHistoryItem = value;
                OnPropertyChanged();
            }
        }

        public bool IsAdsVisible
        {
            get => _isAdsVisible;
            set
            {
                _isAdsVisible = value;
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

        public string LastHistoryItemProtein
        {
            get => _lastHistoryItemProtein;
            set
            {
                _lastHistoryItemProtein = value;
                OnPropertyChanged();
            }
        }

        public string LastHistoryItemFat
        {
            get => _lastHistoryItemFat;
            set
            {
                _lastHistoryItemFat = value;
                OnPropertyChanged();
            }
        }

        public string LastHistoryItemCarbs
        {
            get => _lastHistoryItemCarbs;
            set
            {
                _lastHistoryItemCarbs = value;
                OnPropertyChanged();
            }
        }

        public string LastHistoryItemSugar
        {
            get => _lastHistoryItemSugar;
            set
            {
                _lastHistoryItemSugar = value;
                OnPropertyChanged();
            }
        }



        //public string DishName
        //{
        //    get => "Name: " + _mealName;
        //    set
        //    {
        //        if (_mealName != value)
        //        {

        //            _mealName = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public string Weight
        //{
        //    get => "Weight: " + _weight + "g";
        //    set
        //    {
        //        if (_weight != value)
        //        {

        //            _weight = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public string Calories
        //{
        //    get => "Calories: " + _calories + "Cal";
        //    set
        //    {
        //        if (_calories != value)
        //        {
        //            _calories = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public string Proteins
        //{
        //    get => "Protein: " + _proteins + "g";
        //    set
        //    {
        //        if (_proteins != value)
        //        {
        //            _proteins = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public string Fats
        //{
        //    get => "Fat: " + _fats + "g";
        //    set
        //    {
        //        if (_fats != value)
        //        {
        //            _fats = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public string Carbohydrates
        //{
        //    get => "Carbs: " + _carbohydrates + "g";
        //    set
        //    {
        //        if (_carbohydrates != value)
        //        {
        //            _carbohydrates = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public string TotalResultJSON
        //{
        //    get => _totalResultJSON;
        //    set
        //    {
        //        if (_totalResultJSON != value)
        //        {
        //            _totalResultJSON = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public bool IsRefreshing
        //{
        //    get => _isRefreshing;
        //    set
        //    {
        //        if (_isRefreshing != value)
        //        {
        //            _isRefreshing = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        public bool IsHistoryGridVisible
        {
            get => _isHistoryGridVisible;
            set
            {
                _isHistoryGridVisible = value;
                OnPropertyChanged();
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

            Ingredients = new ObservableCollection<IngredientItem>();
            SaveCommand = new Command(async () => await OnSaveAsync());
            DeleteCommand = new Command(async () => await OnDeleteAsync());
            NewImageCommand = new Command(async () => await OnNewImageAsync());
            LoadAdCommand = new Command(async () => await LoadAdAsync());
            ShowAdCommand = new Command(ShowAd);
        }

        private async Task OnSaveAsync()
        {
            await _navigationService.PopToMainModalAsync();
        }

        private async Task OnDeleteAsync()
        {
            bool delete = await App.Current.MainPage.DisplayAlert("Delete", "Are you sure to delete it?", "Yes", "No");
            if (delete)
            {
                await App.HistoryItemRepository.DeleteMealItemAsync(LastHistoryItem);
                await _navigationService.PopToMainModalAsync();
            }
        }

        private async Task OnNewImageAsync()
        {
            _navigationService.PopModalAsync();
            await _navigationService.NavigateToTakeImagePageAsync();
        }

        #region Process Image

        public async Task ProcessImage() //todo
        {
            try
            {
                IsLoading = true;
                IsHistoryGridVisible = false;
                await LoadSecrets();

                var imagePath = _imageInfo.ImagePath;
                if (imagePath != null)
                {
                    MealItem mealItem = await AnalyzeLocalImage(imagePath);

                    if (mealItem == null)
                    {
                        return;
                    }
                    if (mealItem.IsMeal == false)
                    {
                        _alertService.ShowError("There is no food in this image.");
                        await _navigationService.PopModalAsync();
                        return;
                    }

                    //LoadAIResponse(mealItem);
                    await LoadMealResponse(mealItem);

                    await AddItemToDB(mealItem);
//#if ANDROID
//                    Platform.CurrentActivity.Window.SetFlags(WindowManagerFlags.ForceNotFullscreen, WindowManagerFlags.ForceNotFullscreen);
//#endif
                    
                    IsLoading = false;
                    IsHistoryGridVisible = true;
                    //if (IsAdsVisible == false)
                    //{
                    //    IsHistoryGridVisible = true;
                    //}
                }
            }
            catch (JsonSerializationException)
            {
                _alertService.ShowError("Decoding error occurred.");
                //await _navigationService.PopModalAsync();
                IsLoading = false;
            }
            catch (BadImageFormatException)
            {
                _alertService.ShowError("No connection to AI server");
                //await _navigationService.PopModalAsync();
                IsLoading = false;
            }
            catch (FileLoadException)
            {
                _alertService.ShowError("Error saving to database");
                IsLoading = false;
            }
            catch (Exception)
            {
                _alertService.ShowUnexpectedError();
                //await _navigationService.PopModalAsync();
                IsLoading = false;
            }
        }

        //public void LoadAIResponse(MealItem mealItem)
        //{
        //    DishName = mealItem.MealName;
        //    Weight = mealItem.Weight.ToString();
        //    Calories = ((IMealItem)mealItem).Calories.ToString();
        //    Proteins = mealItem.Proteins.ToString();
        //    Fats = ((IMealItem)mealItem).Fats.ToString();
        //    Carbohydrates = mealItem.Carbohydrates.ToString();
        //    TotalResultJSON = mealItem.TotalResultJSON;
        //    IsRefreshing = false;
        //}

        //public void LoadAIResponse(string response)
        //{
        //    DishName = response;
        //    IsRefreshing = false;
        //}


        public async Task LoadMealResponse(MealItem mealItem)
        {
            try
            {
                LastHistoryItem = mealItem;
                LastHistoryItemImage = mealItem.ImagePath;
                LastHistoryItemName = mealItem.MealName;
                LastHistoryItemCalories = mealItem.Calories.ToString();
                LastHistoryItemCalories = mealItem.Calories.ToString();
                LastHistoryItemProtein = mealItem.Proteins.ToString();
                LastHistoryItemFat = mealItem.Fats.ToString();
                LastHistoryItemCarbs = mealItem.Carbohydrates.ToString();
                LastHistoryItemSugar = mealItem.Sugar.ToString();

                Ingredients = new ObservableCollection<IngredientItem>(mealItem.Ingredients);

                //await LoadLastMealIngredients(mealItem.Id);
            }
            catch (Exception)
            {
                _alertService.ShowError("Loading error occurred");
            }
        }


        //private async Task LoadLastMealIngredients(int lastMealId)
        //{
        //    var lastMealIngredients = await App.IngredientItemRepository.GetIngredientsByMealIdAsync(lastMealId);

        //    Ingredients.Clear();
        //    foreach (var ingredient in lastMealIngredients)
        //    {
        //        Ingredients.Add(ingredient);
        //    }
        //}



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
                mealItem.ImagePath = imagePath;
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

        private async Task AddItemToDB(MealItem mealItem)
        {
            try
            {
                var dateTimeNow = DateTime.Now;

                mealItem.Date = dateTimeNow;
                mealItem.Time = dateTimeNow.ToString("HH:mm");

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

                _apiKeys = new ApiKeys();
                _apiKeys.OpenAIAPIKey = "sk-proj-QgMA" + "bggM8w9pUhiyP2BvT3BlbkFJ" + "Cp1pm1hywYBWL1QNkG1M";
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
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Ads

        private async Task LoadAdAsync()
        {
            try
            {
                var adRequest = new AdRequest.Builder().Build();
                var context = Android.App.Application.Context;


                InterstitialAd.Load(context, _adUnitId, adRequest,
                    new CustomInterstitialAdLoadCallback(
                        ad =>
                        {
                            SetInterstitialAd(ad);
                            Console.WriteLine("Interstitial Ad loaded successfully.");
                            ShowAd();
                        },
                        loadAdError =>
                        {
                            Console.WriteLine($"Failed to load interstitial ad: {loadAdError.Message}");
                        }));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading ads: {ex.Message}");
            }
        }

        public void ShowAd()
        {
            var context = Platform.CurrentActivity;

            if (_interstitialAd != null)
            {
                _interstitialAd.Show(context);
            }
            else
            {
                Console.WriteLine("Ad is not ready to be shown yet.");
            }
        }

        public void SetInterstitialAd(InterstitialAd ad)
        {
            if (ad != null)
            {
                _interstitialAd = ad;
                _interstitialAd.FullScreenContentCallback = new CustomFullScreenContentCallback(this);
            }
        }

        //private Activity? GetCurrentActivity()
        //{
        //    return Platform.CurrentActivity as Activity;
        //}


        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

