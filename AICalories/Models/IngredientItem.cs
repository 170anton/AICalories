using System;
using AICalories.Interfaces;
using Newtonsoft.Json;
using SQLite;

namespace AICalories.Models
{
    public class IngredientItem : IIngredientItem
    {
        private string name;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MealItemId { get; set; }

        [JsonProperty("ingredient_name")]
        public string Name { get => name; set => name = char.ToUpper(value[0]) + value.Substring(1); }

        [JsonProperty("ingredient_weight")]
        public string Weight { get; set; }

        [JsonProperty("ingredient_calories")]
        public string Calories { get; set; }

        [JsonProperty("ingredient_proteins")]
        public double Proteins { get; set; }

        [JsonProperty("ingredient_fats")]
        public double Fats { get; set; }

        [JsonProperty("ingredient_carbohydrates")]
        public double Carbohydrates { get; set; }

        [JsonProperty("ingredient_sugar")]
        public double Sugar { get; set; }
    }
}

