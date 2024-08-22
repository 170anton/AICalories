using AICalories.Models;

namespace AICalories.Interfaces
{
    public interface IMealItem
    {
        int Id { get; set; }
        bool IsMeal { get; set; }
        string MealName { get; set; }
        DateTime Date { get; set; }
        string Time { get; set; }
        string ImagePath { get; set; }
        int Weight { get; set; }
        int Calories { get; set; }
        int Proteins { get; set; }
        int Fats { get; set; }
        int Carbohydrates { get; set; }
        string TotalResultJSON { get; set; }
        List<IngredientItem> Ingredients { get; set; }
    }
}