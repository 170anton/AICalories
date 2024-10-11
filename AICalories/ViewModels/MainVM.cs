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
    private string _todayDate;
    private bool _isLoading;
    private bool _isMakeFirstRecordVisible;
    private bool _isHistoryGridVisible;
    private bool _isPfcsInfoGridVisible;
    private bool _isYesterdayVisible;
    private bool _isGoalsSet;
    private int _dailyCalorieGoal;
    private int _dailyProteinGoal;
    private int _dailyFatGoal;
    private int _dailyCarbsGoal;
    private int _dailySugarGoal;

    private readonly IViewModelService _viewModelService;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;
    private ObservableCollection<IngredientItem> _lastMealIngredients;

    public ContextVM ContextVM => _viewModelService.ContextVM;
    public AppSettingsVM AppSettingsVM => _viewModelService.AppSettingsVM;

    public ICommand NewImageCommand { get; }
    public ICommand DeleteLastMealCommand { get; }
    public ICommand ShowMoreTodayStatsCommand { get; }
    public ICommand SetGoalCommand { get; }

    #region Properties

    public ObservableCollection<MealItem> TodayMealsCollection { get; private set; }

    public ObservableCollection<IngredientItem> LastMealIngredients
    {
        get => _lastMealIngredients;
        set
        {
            _lastMealIngredients = value;
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

    public string TodayDate
    {
        get => _todayDate;
        set
        {
            _todayDate = value;
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

    public bool IsYesterdayVisible
    {
        get => _isYesterdayVisible;
        set
        {
            _isYesterdayVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsGoalsSet
    {
        get => _isGoalsSet;
        set
        {
            _isGoalsSet = value;
            OnPropertyChanged();
        }
    }

    public int DailyCalorieGoal
    {
        get => _dailyCalorieGoal;
        set
        {
            _dailyCalorieGoal = value;
            OnPropertyChanged();
        }
    }

    public int DailyProteinGoal
    {
        get => _dailyProteinGoal;
        set
        {
            _dailyProteinGoal = value;
            OnPropertyChanged();
        }
    }

    public int DailyFatGoal
    {
        get => _dailyFatGoal;
        set
        {
            _dailyFatGoal = value;
            OnPropertyChanged();
        }
    }

    public int DailyCarbsGoal
    {
        get => _dailyCarbsGoal;
        set
        {
            _dailyCarbsGoal = value;
            OnPropertyChanged();
        }
    }

    public int DailySugarGoal
    {
        get => _dailySugarGoal;
        set
        {
            _dailySugarGoal = value;
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

    public bool IsMakeFirstRecordVisible
    {
        get => _isMakeFirstRecordVisible;
        set
        {
            _isMakeFirstRecordVisible = value;
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
        SetGoalCommand = new Command(async () => await SetGoalAsync());

        LastMealIngredients = new ObservableCollection<IngredientItem>();
        TodayMealsCollection = new ObservableCollection<MealItem>();

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
            await LoadTodayStats();

            await LoadLastMeals();

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

        TodayDate = dateTimeNow.ToString("dddd, dd MMMM");
        await GetTotalCalories(items, dateTimeNow);
        await GetTotalProteins(items, dateTimeNow);
        await GetTotalFats(items, dateTimeNow);
        await GetTotalCarbohydrates(items, dateTimeNow);
        await GetTotalSugar(items, dateTimeNow);
        CheckPFCSInfoExist();
        await GetTodayGoals();
    }

    public async Task GetTodayGoals()
    {
        IsGoalsSet = Preferences.Get(App.IsGoalsSet, false);
        DailyCalorieGoal = Preferences.Get(App.DailyCalorieGoal, 0);
        DailyProteinGoal = Preferences.Get(App.DailyProteinGoal, 0);
        DailyFatGoal = Preferences.Get(App.DailyFatGoal, 0);
        DailyCarbsGoal = Preferences.Get(App.DailyCarbsGoal, 0);
        DailySugarGoal = Preferences.Get(App.DailySugarGoal, 0);
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

    private void CheckPFCSInfoExist()
    {
        if (Convert.ToInt32(TotalProteins) == 0 && Convert.ToInt32(TotalCarbohydrates) == 0)
        {
            IsPfcsGridVisible = false;
        }
        else
        {
            IsPfcsGridVisible = true;
        }
    }

    private void LoadShowMoreTodayStatsOption()
    {
        var savedOption = Preferences.Get(App.ShowMoreTodayStatsKey, false);
        if (!savedOption)
        {
            ShowMoreTodayStatsText = "+";
            IsPfcsGridVisible = savedOption;
        }
        else
        {
            ShowMoreTodayStatsText = "-";
            IsPfcsGridVisible = savedOption;
        }
    }

    #endregion

    public async Task LoadLastMeals()
    {
        try
        {
            IsMakeFirstRecordVisible = false;
            IsYesterdayVisible = false;
            IsLoading = true;

            var dateTimeNow = DateTime.Now;
            //await App.HistoryItemRepository.DeleteAllMealItemsAsync();
            var allMealsList = await App.HistoryItemRepository.GetAllMealItemsAsync();

            if (allMealsList.Count == 0)
            {
                IsHistoryGridVisible = false;
                IsLoading = false;
                IsMakeFirstRecordVisible = true;

                return;
            }

            var mealListByDay = allMealsList.Where(i => i.Date.Date == dateTimeNow.Date)
                               .OrderByDescending(g => g.Date)
                               .ToList();

            if (mealListByDay.Count() == 0)
            {
                mealListByDay = await SetYesterdayInfo(dateTimeNow, allMealsList, mealListByDay);
            }

            if (CheckOnUpdate(dateTimeNow, mealListByDay))
            {
                await FillCollection(mealListByDay);
            }


            IsLoading = false;
            IsHistoryGridVisible = true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    private async Task<List<MealItem>> SetYesterdayInfo(DateTime dateTimeNow, List<MealItem> items, List<MealItem> todayMeals)
    {
        var yesterday = dateTimeNow.AddDays(-1);
        var yesterdayMeals = items.Where(i => i.Date.Date == yesterday.Date)
                       .OrderByDescending(g => g.Date)
                       .ToList();

        if (yesterdayMeals.Count() != 0)
        {
            IsYesterdayVisible = true;
            return yesterdayMeals;
        }
        else
            return todayMeals;
    }

    private async Task FillCollection(List<MealItem> mealList)
    {
        TodayMealsCollection.Clear();

        foreach (var meal in mealList)
        {
            await LoadMealIngredients(meal);
            TodayMealsCollection.Add(meal);
        }
    }

    /// <summary>
    /// Returns true if collection should be updated
    /// </summary>
    /// <param name="dateTimeNow"></param>
    /// <param name="todayMeals"></param>
    /// <returns></returns>
    private bool CheckOnUpdate(DateTime dateTimeNow, List<MealItem> todayMeals)
    {
        bool haveSameImages = todayMeals.Select(i => i.ImagePath)
                               .SequenceEqual(TodayMealsCollection.ToList().Select(i => i.ImagePath));

        if (haveSameImages)
            return false;
        else
            return true;
    }


    private async Task LoadLastMealIngredients(int lastMealId)
    {
        var lastMealIngredients = await App.IngredientItemRepository.GetIngredientsByMealIdAsync(lastMealId);

        //Ingredients.Clear();
        LastMealIngredients = new ObservableCollection<IngredientItem>(lastMealIngredients);
        //foreach (var ingredient in lastMealIngredients)
        //{
        //    Ingredients.Add(ingredient);
        //}
    }

    private async Task LoadMealIngredients(IMealItem mealItem)
    {
        mealItem.Ingredients = await App.IngredientItemRepository.GetIngredientsByMealIdAsync(mealItem.Id);
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

    private async Task SetGoalAsync()
    {
        string caloriesInput;
        string proteinInput;
        string fatInput;
        string carbsInput;
        string sugarInput;

        caloriesInput = await Application.Current.MainPage.DisplayPromptAsync(
            "Set Calorie Goal",
            "Enter your daily calorie goal:",
            keyboard: Keyboard.Numeric
        );
        proteinInput = await Application.Current.MainPage.DisplayPromptAsync(
            "Set Protein Goal",
            "Enter your daily protein goal (in grams):",
            keyboard: Keyboard.Numeric
        );
        fatInput = await Application.Current.MainPage.DisplayPromptAsync(
                    "Set Fat Goal",
                    "Enter your daily fat goal (in grams):",
                    keyboard: Keyboard.Numeric
                );
        carbsInput = await Application.Current.MainPage.DisplayPromptAsync(
            "Set Carbs Goal",
            "Enter your daily carbs goal (in grams):",
            keyboard: Keyboard.Numeric
        );

        sugarInput = await Application.Current.MainPage.DisplayPromptAsync(
            "Set Sugar Goal",
            "Enter your daily sugar goal (in grams):",
            keyboard: Keyboard.Numeric
        );

        //string nutrientsInput = await Application.Current.MainPage.DisplayPromptAsync(
        //    "Set Nutrient Goal",
        //    "Enter your daily nutrient goal (e.g., protein, carbs, fats):",
        //    keyboard: Keyboard.Text
        //);

        if (true) //todo
        {
            DailyCalorieGoal = Convert.ToInt32(caloriesInput); ;
            DailyProteinGoal = Convert.ToInt32(proteinInput);
            DailyFatGoal = Convert.ToInt32(fatInput); ;
            DailyCarbsGoal = Convert.ToInt32(carbsInput); ;
            DailySugarGoal = Convert.ToInt32(sugarInput); ;

            Preferences.Set(App.DailyCalorieGoal, DailyCalorieGoal);
            Preferences.Set(App.DailyProteinGoal, DailyProteinGoal);
            Preferences.Set(App.DailyFatGoal, DailyFatGoal);
            Preferences.Set(App.DailyCarbsGoal, DailyCarbsGoal);
            Preferences.Set(App.DailySugarGoal, DailySugarGoal);

            Preferences.Set(App.IsGoalsSet, true);

            await Application.Current.MainPage.DisplayAlert("Success", "Goals have been set!", "OK");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Invalid Input", "Please enter valid numeric values for calories.", "OK");
        }
    }

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