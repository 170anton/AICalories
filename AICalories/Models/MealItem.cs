using System;
using System.Collections.ObjectModel;
using AICalories.Interfaces;
using SQLite;

namespace AICalories.Models
{
    public class MealItem : IMealItem
    {
        private string calories;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string ImagePath { get; set; }
        //public string Calories { get => calories; set => calories = value; } //+ " cals"
        public int Calories { get; set; }
        public int Proteins { get; set; }
        public int Fats { get; set; }
        public int Carbohydrates { get; set; }

    }
}

