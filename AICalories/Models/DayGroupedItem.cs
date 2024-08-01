using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SQLite;

namespace AICalories.Models
{
    public class DayGroupedItem : ObservableCollection<HistoryItem>
    {
        public string Title { get; protected set; }
        public DateTime Date { get; protected set; }


        public DayGroupedItem(DateTime date)
        {
            Date = date;
            Title = date.ToString("dd MMMM");
        }

    }
}

