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
        Task SaveMealItemAsync(MealItem item);
    }
}