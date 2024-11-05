using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Models;
using AICalories.Services;
using AICalories.Views;

namespace AICalories.ViewModels
{
    public class HistoryVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        private DateTime _selectedMonth;
        private bool _isMealClicked;
        private bool _isLoading;
        private bool _isLabelVisible;
        private bool _isLayoutVisible;

        public ObservableCollection<DayGroupedItem> DayGroupedItems { get; private set; }
        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand OpenMealInfoCommand { get; }

        #region Properties

        public DateTime SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
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


        public bool IsLayoutVisible
        {
            get => _isLayoutVisible;
            set
            {
                _isLayoutVisible = value;
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

        public HistoryVM(IViewModelService viewModelService,
            INavigationService navigationService, IAlertService alertService)
        {
            _viewModelService = viewModelService;
            _viewModelService.HistoryVM = this;
            _navigationService = navigationService;
            _alertService = alertService;

            //OnClearAll();
            DayGroupedItems = new ObservableCollection<DayGroupedItem>();
            PreviousMonthCommand = new Command(OnPreviousMonth);
            NextMonthCommand = new Command(OnNextMonth);
            AddItemCommand = new Command(OnAddItem);
            DeleteItemCommand = new Command<MealItem>(OnDeleteItem);
            ClearAllCommand = new Command(OnClearAll);
            OpenMealInfoCommand = new Command<MealItem>(OnOpenMealInfo);
            //Task.Run(() => LoadData());

            SelectedMonth = DateTime.Now;
        }

        public async Task OnPageAppearingAsync()
        {
            try
            {
                IsLabelVisible = false;
                IsLoading = true;

                if (await CheckIfDbEmpty())
                {
                    return;
                }

                await SetCurrentMonthAsDate();

                if (!await CheckForMonthUpdate())
                {
                    await UpdateSelectedMonthData();
                } 

                IsLayoutVisible = true;
                IsLoading = false;
            }
            catch (Exception)
            {
                _alertService.ShowError("Loading error occurred");
            }
        }

        private async Task<bool> CheckIfDbEmpty()
        {
            var countInDb = await App.HistoryItemRepository.GetMealItemsCountAsync();

            if (countInDb == 0)
            {
                IsLoading = false;
                IsLayoutVisible = false;
                IsLabelVisible = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private Task SetCurrentMonthAsDate()
        {
            SelectedMonth = DateTime.Now;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns false if Collection should be updated
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckForMonthUpdate()
        {
            try
            {
                var itemsInDb = await App.HistoryItemRepository.GetAllMealItemsByMonthAsync(SelectedMonth);
                //var filteredItemsInDb = itemsInDb
                //    .Where(item => item.Date.Month == SelectedMonth.Month && item.Date.Year == SelectedMonth.Year)
                //    .ToList();

                var itemsInCollection = DayGroupedItems.SelectMany(grouped => grouped);
                //var filteredItemsInCollection = itemsInCollection
                //    .Where(item => item.Date.Month == SelectedMonth.Month && item.Date.Year == SelectedMonth.Year)
                //    .ToList();

                return itemsInDb.SequenceEqual(itemsInCollection);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CheckForUpdate: {ex.Message}");
                _alertService.ShowUnexpectedError();
                return true;
            }
        }

        public async Task UpdateSelectedMonthData()
        {
            try
            {
                IsLoading = true;
                DayGroupedItems.Clear();

                //var dateTimeNow = SelectedMonth;
                var items = await App.HistoryItemRepository.GetAllMealItemsByMonthAsync(SelectedMonth);

                var grouped = items.GroupBy(i => i.Date.Date)
                                   .Select(g => new DayGroupedItem(g.Key))
                                   .OrderByDescending(g => g.Date)
                                   .ToList();

                foreach (var group in grouped)
                {
                    foreach (var item in items)
                    {
                        if (item.Date.Date == group.Date.Date)
                        {
                            group.Add(item);
                        }
                    }

                    group.TotalCalories = group.Sum(i => i.Calories);

                    DayGroupedItems.Add(group);
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Sad");
            }

        }

        private async void OnPreviousMonth()
        {
            try
            {
                if (await IsInHistoryBoundDateRange(SelectedMonth.AddMonths(-1)))
                {
                    SelectedMonth = SelectedMonth.AddMonths(-1);
                    await UpdateSelectedMonthData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _alertService.ShowUnexpectedError();
            }
        }

        private async void OnNextMonth()
        {
            try
            {
                if (await IsInHistoryBoundDateRange(SelectedMonth.AddMonths(1)))
                {
                    SelectedMonth = SelectedMonth.AddMonths(1);
                    await UpdateSelectedMonthData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _alertService.ShowUnexpectedError();
            }
        }

        private async Task<bool> IsInHistoryBoundDateRange(DateTime selectedMonth)
        {
            var lowerBoundItem = await App.HistoryItemRepository.GetOldestMealItemAsync();
            var upperBoundItem = await App.HistoryItemRepository.GetLastMealItemAsync();

            // Define if selectedMonth is within the bounds of the history range
            bool isAfterLowerBound = selectedMonth.Year > lowerBoundItem.Date.Year ||
                                     (selectedMonth.Year == lowerBoundItem.Date.Year && selectedMonth.Month >= lowerBoundItem.Date.Month);

            bool isBeforeUpperBound = selectedMonth.Year < upperBoundItem.Date.Year ||
                                      (selectedMonth.Year == upperBoundItem.Date.Year && selectedMonth.Month <= upperBoundItem.Date.Month);

            // Return true if within range, false otherwise
            return isAfterLowerBound && isBeforeUpperBound;
        }

        public async Task CheckForUpdate()
        {
            try
            {
                var countInDb = await App.HistoryItemRepository.GetMealItemsCountAsync(); //todo Replace
                var countInColl = DayGroupedItems.SelectMany(grouped => grouped).Count();

                if (countInDb != countInColl)
                {
                    await UpdateData();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CheckForUpdate: {ex.Message}");
                _alertService.ShowUnexpectedError();
            }
        }

        public async Task UpdateData()
        {
            try
            {
                IsLoading = true;
                DayGroupedItems.Clear();

                var dateTimeNow = DateTime.Now;
                var items = await App.HistoryItemRepository.GetAllMealItemsAsync();

                var grouped = items.GroupBy(i => i.Date.Date)
                                   .Select(g => new DayGroupedItem(g.Key))
                                   .OrderByDescending(g => g.Date)
                                   .ToList();

                foreach (var group in grouped)
                {
                    foreach (var item in items)
                    {
                        if (item.Date.Date == group.Date.Date)
                        {
                            group.Add(item);
                        }
                    }

                    group.TotalCalories = group.Sum(i => i.Calories);

                    DayGroupedItems.Add(group);
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Sad");
            }
            
        }

        private async void OnAddItem()
        {
            try
            {
                var dateTimeNow = DateTime.Now;
                var newItem = new MealItem
                {
                    Date = dateTimeNow,
                    Time = dateTimeNow.ToString("HH:mm"),
                    ImagePath = "pasta1.jpg",
                    Calories = 500
                };
                await App.HistoryItemRepository.SaveMealItemAsync(newItem);
                SaveToHistory(dateTimeNow, newItem);
                //OnPropertyChanged(nameof(DayGroupedItems));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Sad");
            }

        }

        private async void SaveToHistory(DateTime dateTimeNow, MealItem newItem)
        {
            var group = DayGroupedItems.FirstOrDefault(g => g.Date == dateTimeNow.Date);
            if (group == null)
            {
                group = new DayGroupedItem(dateTimeNow.Date);
                DayGroupedItems.Add(group);
                //ObservColl does not sort
                await UpdateData();
            }

            group.Add(newItem);
        }

        private async void OnOpenMealInfo(MealItem mealItem)
        {
            try
            {
                //await _navigationService.NavigateToMealInfoPageAsync(); todo Complete DI for MealInfoPage

                if (_isMealClicked)
                    return;

                _isMealClicked = true;

                mealItem.Ingredients = await App.IngredientItemRepository.GetIngredientsByMealIdAsync(mealItem.Id);

                var mealInfoPage = new MealInfoPage(mealItem);
                await Shell.Current.Navigation.PushModalAsync(mealInfoPage);

                _isMealClicked = false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Sad");
            }
        }

        private async void OnDeleteItem(MealItem item)
        {
            try
            {
                var dateTimeNow = DateTime.Now;
                if (item != null)
                {
                    await App.HistoryItemRepository.DeleteMealItemAsync(item);
                    var group = DayGroupedItems.FirstOrDefault(g => g.Date == item.Date.Date);
                    if (group != null)
                    {
                        group.Remove(item);
                        group.TotalCalories = group.Sum(i => i.Calories);
                        //OnPropertyChanged();
                        if (group.Count == 0)
                        {
                            DayGroupedItems.Remove(group);
                            if (DayGroupedItems.Count() == 0)
                            {
                                IsLabelVisible = true;
                            }
                        }

                        //OnPropertyChanged(nameof(DayGroupedItems));
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Sad");
            }

        }

        private async void OnClearAll()
        {
            try
            {
                await App.HistoryItemRepository.DeleteAllMealItemsAsync();
                DayGroupedItems.Clear();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Sad");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

