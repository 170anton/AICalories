using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AICalories.ViewModels;

public class MainVM : INotifyPropertyChanged
{
    private MealItem _lastHistoryItem;
    private string _lastHistoryItemImage;
    private string _lastHistoryItemName;
    private string _lastHistoryItemCalories;
    private string _lastHistoryItemProtein;
    private string _lastHistoryItemFat;
    private string _lastHistoryItemCarbs;
    private string _lastHistoryItemSugar;
    private string _totalCalories;
    private string _totalProteins;
    private string _totalFats;
    private string _totalCarbohydrates;
    private string _totalSugar;
    private string _showMoreTodayStatsText;
    private bool _isLoading;
    private bool _isLabelVisible;
    private bool _isHistoryGridVisible;
    private bool _isPfcsInfoGridVisible;

    private readonly IViewModelService _viewModelService;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;
    private ObservableCollection<IngredientItem> _ingredients;

    public ContextVM ContextVM => _viewModelService.ContextVM;
    public AppSettingsVM AppSettingsVM => _viewModelService.AppSettingsVM;

    public ICommand NewImageCommand { get; }
    public ICommand DeleteLastMealCommand { get; }
    public ICommand ShowMoreTodayStatsCommand { get; }

    #region Properties

    public ObservableCollection<IngredientItem> Ingredients
    {
        get => _ingredients;
        set
        {
            _ingredients = value;
            OnPropertyChanged();
        }
    }

    public string ShowMoreTodayStatsText
    {
        get => _showMoreTodayStatsText;
        set
        {
            _showMoreTodayStatsText = value;
            OnPropertyChanged();
        }
    }

    public string TotalCalories
    {
        get => _totalCalories;
        set
        {
            _totalCalories = value;
            OnPropertyChanged();
        }
    }

    public string TotalProteins
    {
        get => _totalProteins;
        set
        {
            _totalProteins = value;
            OnPropertyChanged();
        }
    }

    public string TotalFats
    {
        get => _totalFats;
        set
        {
            _totalFats = value;
            OnPropertyChanged();
        }
    }

    public string TotalCarbohydrates
    {
        get => _totalCarbohydrates;
        set
        {
            _totalCarbohydrates = value;
            OnPropertyChanged();
        }
    }

    public string TotalSugar
    {
        get => _totalSugar;
        set
        {
            _totalSugar = value;
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

    public bool IsHistoryGridVisible
    {
        get => _isHistoryGridVisible;
        set
        {
            _isHistoryGridVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsPfcsGridVisible
    {
        get => _isPfcsInfoGridVisible;
        set
        {
            _isPfcsInfoGridVisible = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Constructor

    public MainVM(IViewModelService viewModelService,
            INavigationService navigationService, IAlertService alertService)
    {
        _viewModelService = viewModelService;
        _viewModelService.MainVM = this;
        _navigationService = navigationService;
        _alertService = alertService;

        NewImageCommand = new Command(async () => await NewImageClicked());
        DeleteLastMealCommand = new Command(async () => await DeleteLastMealClicked());
        ShowMoreTodayStatsCommand = new Command(ShowMoreTodayStatsClicked);

        Ingredients = new ObservableCollection<IngredientItem>();

        LoadShowMoreTodayStatsOption();

        //LoadLastHistoryItem();
        //GetTotalCalories();
        //GetTotalProteins();
        //GetTotalFats();
        //GetTotalCarbohydrates();
        //IsHistoryGridVisible = true;

    }
    #endregion

    public async Task OnPageAppearingAsync()
    {
        try
        {
            //LastHistoryItemName = null;
            //LastHistoryItemCalories = null;
            //LastHistoryItemImage = null;

            await LoadTodayStats();

            await LoadLastMeal();
        }
        catch (Exception)
        {
            _alertService.ShowError("Loading error occurred");
        }
    }

    #region Today Stats

    public async Task LoadTodayStats()
    {

        var dateTimeNow = DateTime.Now;
        List<MealItem> items = await App.HistoryItemRepository.GetAllMealItemsAsync();

        await GetTotalCalories(items, dateTimeNow);
        await GetTotalProteins(items, dateTimeNow);
        await GetTotalFats(items, dateTimeNow);
        await GetTotalCarbohydrates(items, dateTimeNow);
        await GetTotalSugar(items, dateTimeNow);
    }

    public async Task GetTotalCalories(List<MealItem> items, DateTime dateTimeNow)
    {
        var calorieSum = items.Where(i => i.Date.Date == dateTimeNow.Date)
                              .Sum(i => i.Calories)
                              .ToString();

        TotalCalories = calorieSum;
    }

    public async Task GetTotalProteins(List<MealItem> items, DateTime dateTimeNow)
    {
        var proteinsSum = items.Where(i => i.Date.Date == dateTimeNow.Date)
                              .Sum(i => i.Proteins)
                              .ToString();

        TotalProteins = proteinsSum;
    }

    public async Task GetTotalFats(List<MealItem> items, DateTime dateTimeNow)
    {
        var fatsSum = items.Where(i => i.Date.Date == dateTimeNow.Date)
                              .Sum(i => i.Fats)
                              .ToString();

        TotalFats = fatsSum;
    }

    public async Task GetTotalCarbohydrates(List<MealItem> items, DateTime dateTimeNow)
    {
        var carbohydratesSum = items.Where(i => i.Date.Date == dateTimeNow.Date)
                              .Sum(i => i.Carbohydrates)
                              .ToString();

        TotalCarbohydrates = carbohydratesSum;
    }

    public async Task GetTotalSugar(List<MealItem> items, DateTime dateTimeNow)
    {
        var sugarSum = items.Where(i => i.Date.Date == dateTimeNow.Date)
                              .Sum(i => i.Sugar)
                              .ToString();

        TotalSugar = sugarSum;
    }


    private void ShowMoreTodayStatsClicked()
    {
        Preferences.Set(App.ShowMoreTodayStatsKey, !Preferences.Get(App.ShowMoreTodayStatsKey, false));
        LoadShowMoreTodayStatsOption();
    }

    private void LoadShowMoreTodayStatsOption()
    {
        var savedOption = Preferences.Get(App.ShowMoreTodayStatsKey, false);
        if (!savedOption)
        {
            ShowMoreTodayStatsText = "show more";
            IsPfcsGridVisible = savedOption;
        }
        else
        {
            ShowMoreTodayStatsText = "show less";
            IsPfcsGridVisible = savedOption;
        }
    }

    #endregion

    public async Task LoadLastMeal()
    {
        try
        {
            IsLabelVisible = false;
            IsLoading = true;
            //await Task.Delay(500);
            var lastMeal = await App.HistoryItemRepository.GetLastMealItemAsync();
            IsLoading = false;

            if (lastMeal == null)
            {
                IsHistoryGridVisible = false;
                IsLabelVisible = true;
                return;
            }

            if (LastHistoryItemImage != lastMeal.ImagePath || LastHistoryItemName != lastMeal.MealName) //todo test in release
            {
                _lastHistoryItem = lastMeal;
                LastHistoryItemImage = lastMeal.ImagePath;
                LastHistoryItemName = lastMeal.MealName;
                LastHistoryItemCalories = lastMeal.Calories.ToString();
                LastHistoryItemProtein = lastMeal.Proteins.ToString();
                LastHistoryItemFat = lastMeal.Fats.ToString();
                LastHistoryItemCarbs = lastMeal.Carbohydrates.ToString();
                LastHistoryItemSugar = lastMeal.Sugar.ToString();

                await LoadLastMealIngredients(lastMeal.Id);
            }

            IsHistoryGridVisible = true;

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

        //Ingredients.Clear();
        Ingredients = new ObservableCollection<IngredientItem>(lastMealIngredients);
        //foreach (var ingredient in lastMealIngredients)
        //{
        //    Ingredients.Add(ingredient);
        //}
    }

    #region Photo selection buttons

    private async Task NewImageClicked()
    {
        try
        {
            bool isCameraAvailable = await CheckAndRequestCameraPermissionAsync();
            if (isCameraAvailable)
            {
                try
                {
                    //var image = await MediaPicker.Default.CapturePhotoAsync();
                    //await CrossMedia.Current.Initialize();

                    //var takeImagePage = new TakeImagePage();

                    if (!InternetConnection.CheckInternetConnection())
                    {
                        DisplayAlertConfiguration.ShowError("No internet connection");
                        return;
                    }

                    _navigationService.PopModalAsync();
                    await _navigationService.NavigateToTakeImagePageAsync();

                    //var takeImagePage = IPlatformApplication.Current.Services.GetService<TakeImagePage>();
                    //await Shell.Current.Navigation.PushModalAsync(takeImagePage);


                    //await CrossMedia.Current.Initialize();

                    //MediaFile image = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    //{
                    //    PhotoSize = PhotoSize.Medium,
                    //    //SaveToAlbum = true
                    //});

                }
                catch (ArgumentNullException ex)
                {
                    DisplayAlertConfiguration.ShowError("No connection to AI server. Please try again later");
                }
                catch (Exception ex)
                {
                    DisplayAlertConfiguration.ShowError($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Permission Denied", "Camera permission is required to take photos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "Sad");
        }
    }

    private async Task<bool> CheckAndRequestCameraPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            // Handle the exception as needed
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            return false;
        }
    }

    #endregion



    private async Task DeleteLastMealClicked()
    {
        bool delete = await App.Current.MainPage.DisplayAlert("Delete", "Are you sure to delete it?", "Yes", "No");
        if (delete)
        {
            await App.HistoryItemRepository.DeleteMealItemAsync(_lastHistoryItem);
            await OnPageAppearingAsync();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}