using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SQLite;

namespace AICalories.Models
{
    public class DayGroupedItem : ObservableCollection<MealItem>
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int TotalCalories { get; set; }


        public DayGroupedItem(DateTime date)
        {
            Date = date;
            Title = date.ToString("dd MMMM");
        }

    }
}

