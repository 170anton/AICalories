using System;
using AICalories.Interfaces;
using SQLite;

namespace AICalories.Models
{
    public class IngredientItem : IIngredientItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MealItemId { get; set; }
        public string Name { get; set; }
        public string Weight { get; set; }
        public string Calories { get; set; }
    }
}

