using AICalories.Models;

namespace AICalories.Repositories
{
    public interface IHistoryItemRepository
    {
        Task DeleteMealItemAsync(MealItem item);
        Task<List<MealItem>> GetAllMealItemsAsync();
        Task DeleteAllMealItemsAsync();
        Task<MealItem> GetLastMealItemAsync();
        Task<int> GetMealItemsCountAsync();
        Task UpdateMealItemAsync(MealItem item);
        Task SaveMealItemAsync(MealItem item);
    }
}