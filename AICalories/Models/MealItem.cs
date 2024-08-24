using System;
using System.Collections.ObjectModel;
using AICalories.Interfaces;
using Newtonsoft.Json;
using SQLite;

namespace AICalories.Models
{
    public class MealItem : IMealItem
    {
        private string mealName;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonProperty("is_meal")]
        public bool IsMeal { get; set; }

        [JsonProperty("meal_name")]
        public string MealName { get => mealName; set => mealName = char.ToUpper(value[0]) + value.Substring(1); }

        public DateTime Date { get; set; }

        public string Time { get; set; }

        public string ImagePath { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("calories")]
        public int Calories { get; set; }

        [JsonProperty("proteins")]
        public int Proteins { get; set; }

        [JsonProperty("fats")]
        public int Fats { get; set; }

        [JsonProperty("carbohydrates")]
        public int Carbohydrates { get; set; }

        public string TotalResultJSON { get; set; } //todo remove

        [Ignore]
        [JsonProperty("ingredients")]
        public List<IngredientItem> Ingredients { get; set; }

    }
}

