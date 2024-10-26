using System;
using AICalories.Models;
using System.Collections.ObjectModel;
using AICalories.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Services;

namespace AICalories.ViewModels
{
	public class MealInfoVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        private IImageInfo _imageInfo;


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

        public ICommand DeleteSelectedIngredientCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

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

        public MealInfoVM(IViewModelService viewModelService, IImageInfo imageInfo,
            INavigationService navigationService, IAlertService alertService)
		{
            _viewModelService = viewModelService;
            _viewModelService.MealInfoVM = this;
            _navigationService = navigationService;
            _alertService = alertService;
            _imageInfo = imageInfo;


            DeleteSelectedIngredientCommand = new Command<IngredientItem>(DeleteSelectedIngredientClicked);
            SaveCommand = new Command(async () => await OnSaveAsync());
            DeleteCommand = new Command(async () => await OnDeleteMealAsync());


        }


        public async Task LoadMealInfo(MealItem mealItem)
        {
            try
            {
                IsLoading = true;

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

                IsHistoryGridVisible = true;
                IsLoading = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _alertService.ShowError("Loading error occurred");
            }
        }

        private async Task OnSaveAsync()
        {
            await _navigationService.PopModalAsync();
        }

        private async Task OnDeleteMealAsync()
        {
            bool delete = await App.Current.MainPage.DisplayAlert("Delete", "Are you sure to delete it?", "Yes", "No");
            if (delete)
            {
                await App.HistoryItemRepository.DeleteMealItemAsync(LastHistoryItem);
                await _navigationService.PopModalAsync();
            }
        }

        private async void DeleteSelectedIngredientClicked(IngredientItem item)
        {
            try
            {
                bool delete = await App.Current.MainPage.DisplayAlert("Delete", "Are you sure to delete this ingredient?", "Yes", "No");
                if (delete)
                {
                    if (item != null)
                    {
                        await DeleteMealIngredient(item);
                        await UpdateMealCalories(item);
                        await UpdateMealDB();
                        _viewModelService.HistoryVM.UpdateData();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _alertService.ShowUnexpectedError();
            }
        }


        private async Task DeleteMealIngredient(IngredientItem item)
        {
            Ingredients.Remove(item);
            LastHistoryItem.Ingredients.Remove(item);

            await App.IngredientItemRepository.DeleteIngredientAsync(item);
        }

        private async Task UpdateMealCalories(IngredientItem item)
        {
            LastHistoryItem.Calories -= Convert.ToInt32(item.Calories);

            if (LastHistoryItem.Calories < 0)
            {
                LastHistoryItem.Calories = 0;
            }

            LastHistoryItemCalories = LastHistoryItem.Calories.ToString();
        }

        private async Task UpdateMealDB()
        {
            await App.HistoryItemRepository.UpdateMealItemAsync(LastHistoryItem);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

