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
        private double weight;
        private double calories;
        private double proteins;
        private double fats;
        private double carbohydrates;
        private double sugar;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonProperty("is_food")]
        public bool IsMeal { get; set; }

        [JsonProperty("meal_name")]
        public string MealName { get => mealName; set => mealName = value; }     //char.ToUpper(value[0]) + value.Substring(1); }

        public DateTime Date { get; set; }

        public string Time { get; set; }

        public string ImagePath { get; set; }

        [JsonProperty("weight")]
        public double Weight { get => weight; set => weight = Math.Round(value); }

        [JsonProperty("calories")]
        public double Calories { get => calories; set => calories = Math.Round(value); }

        [JsonProperty("proteins")]
        public double Proteins { get => proteins; set => proteins = Math.Round(value); }

        [JsonProperty("fats")]
        public double Fats { get => fats; set => fats = Math.Round(value); }

        [JsonProperty("carbohydrates")]
        public double Carbohydrates { get => carbohydrates; set => carbohydrates = Math.Round(value); }

        [JsonProperty("sugar")]
        public double Sugar { get => sugar; set => sugar = Math.Round(value); }

        public string TotalResultJSON { get; set; } //todo remove

        [Ignore]
        [JsonProperty("ingredients")]
        public List<IngredientItem> Ingredients { get; set; }

    }
}

