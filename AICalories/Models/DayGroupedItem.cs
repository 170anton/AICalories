using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace AICalories.Models
{
    public class DayGroupedItem : ObservableCollection<MealItem>, INotifyPropertyChanged
    {
        private double _totalCalories;

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public double TotalCalories
        {
            get => _totalCalories;
            set
            {
                _totalCalories = value;
                OnPropertyChanged();
            }
        }


        public DayGroupedItem(DateTime date)
        {
            Date = date;
            Title = date.ToString("dd MMMM");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

