namespace AICalories.Interfaces
{
    public interface IMealItem
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime Date { get; set; }
        string Time { get; set; }
        string ImagePath { get; set; }
        int Calories { get; set; }
        int Proteins { get; set; }
        int Fats { get; set; }
        int Carbohydrates { get; set; }
    }
}