using System;
using System.Collections.ObjectModel;
using SQLite;

namespace AICalories.Models
{
	public class HistoryItem
	{
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string ImagePath { get; set; }
        public string Calories { get; set; }

    }
}

