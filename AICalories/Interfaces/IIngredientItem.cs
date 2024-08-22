namespace AICalories.Interfaces
{
    public interface IIngredientItem
    {
        int Id { get; set; }
        int MealItemId { get; set; }
        string Name { get; set; }
        string Weight { get; set; }
        string Calories { get; set; }
    }
}