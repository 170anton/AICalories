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
        double Weight { get; set; }
        double Calories { get; set; }
        double Proteins { get; set; }
        double Fats { get; set; }
        double Carbohydrates { get; set; }
        string TotalResultJSON { get; set; }
        List<IngredientItem> Ingredients { get; set; }
    }
}