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

    public ContextVM ContextVM => _viewModelService.ContextVM;
    public AppSettingsVM AppSettingsVM => _viewModelService.AppSettingsVM;

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

    public MainVM(IViewModelService viewModelService)
    {
        _viewModelService = viewModelService;
        _viewModelService.MainVM = this;

    }
    #endregion


    public async void GetTotalCalories()
    {

        var dateTimeNow = DateTime.Now;
        var items = await App.Database.GetItemsAsync();
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


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}