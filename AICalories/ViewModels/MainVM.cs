using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Models;
using AICalories.Services;
using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AICalories.ViewModels;

public class MainVM : INotifyPropertyChanged
{
    private string _lastHistoryItemImage;
    private string _lastHistoryItemName;
    private string _lastHistoryItemCalories;
    private string _totalCalories;
    private bool _isLoading;
    private bool _isLabelVisible;
    private bool _isHistoryGridVisible;
    
    private readonly IViewModelService _viewModelService;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;

    public ContextVM ContextVM => _viewModelService.ContextVM;
    public AppSettingsVM AppSettingsVM => _viewModelService.AppSettingsVM;

    public ICommand NewImageCommand { get; }

    #region Properties

    public string TotalCalories
    {
        get => _totalCalories;
        set
        {
            _totalCalories = value;
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
    }
    #endregion


    public async void GetTotalCalories()
    {

        var dateTimeNow = DateTime.Now;
        var items = await App.HistoryDatabase.GetItemsAsync();
        var calorieSum = items.Where(i => i.Date.Date == dateTimeNow.Date)
                              .Sum(i => i.CaloriesInt)
                              .ToString();

        TotalCalories = calorieSum;
    }

    public async void LoadLastHistoryItem()
    {
        try
        {
            IsLabelVisible = false;
            IsLoading = true;
            await Task.Delay(1000);
            var lastItem = await App.HistoryDatabase.GetLastItemAsync();
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


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}