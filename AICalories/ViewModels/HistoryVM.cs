using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.Models;

namespace AICalories.ViewModels
{
	public class HistoryVM : INotifyPropertyChanged
    {
        private bool _isLoading;
        private bool _isLabelVisible;

        public ObservableCollection<DayGroupedItem> DayGroupedItems { get; private set; }
        public ICommand AddItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand ClearAllCommand { get; }

        #region Properties

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
        #endregion

        public HistoryVM()
        {
            //OnClearAll();
            DayGroupedItems = new ObservableCollection<DayGroupedItem>();
            AddItemCommand = new Command(OnAddItem);
            DeleteItemCommand = new Command<MealItem>(OnDeleteItem);
            ClearAllCommand = new Command(OnClearAll);
            //Task.Run(() => LoadData());
        }


        public async void CheckForUpdate()
        {
            try
            {
                IsLabelVisible = false;
                IsLoading = true;
                //await Task.Delay(500);

                var countInDb = await App.HistoryItemRepository.GetMealItemsCountAsync();
                var countInColl = DayGroupedItems.SelectMany(grouped => grouped).Count();

                if (countInDb != countInColl)
                {
                    UpdateData();
                }

                if (countInDb == 0)
                {
                    IsLabelVisible = true;
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CheckForUpdate: {ex.Message}");
                throw;
            }
        }

        public async void UpdateData()
        {
            try
            {
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
                UpdateData();
            }

            group.Add(newItem);
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

