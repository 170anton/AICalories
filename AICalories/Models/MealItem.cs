using System;
using System.Collections.ObjectModel;
using AICalories.Interfaces;
using Newtonsoft.Json;
using SQLite;

namespace AICalories.Models
{
    public class MealItem : IMealItem
    {
        //private string calories;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsMeal { get; set; }
        public string MealName { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string ImagePath { get; set; }
        //public string Calories { get => calories; set => calories = value; } //+ " cals"
        public int Weight { get; set; }
        public int Calories { get; set; }
        public int Proteins { get; set; }
        public int Fats { get; set; }
        public int Carbohydrates { get; set; }
        public string TotalResultJSON { get; set; } //todo remove
        [Ignore]
        public List<IngredientItem> Ingredients { get; set; }

    }
}

