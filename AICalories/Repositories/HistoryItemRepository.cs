using System;
using AICalories.Models;

namespace AICalories.Repositories
{
    public class HistoryItemRepository : IHistoryItemRepository
    {
        private readonly HistoryDatabase<MealItem> _contextDatabase;

        public HistoryItemRepository(HistoryDatabase<MealItem> contextDatabase)
        {
            _contextDatabase = contextDatabase;
        }

        public Task SaveMealItemAsync(MealItem item)
        {
            return _contextDatabase.SaveItemAsync(item);
        }

        public Task<MealItem> GetLastMealItemAsync()
        {
            return _contextDatabase.GetLastItemAsync();
        }

        public Task<List<MealItem>> GetAllMealItemsAsync()
        {
            return _contextDatabase.GetAllItemsAsync();
        }

        public Task<int> GetMealItemsCountAsync()
        {
            return _contextDatabase.GetCountAsync();
        }

        public Task DeleteMealItemAsync(MealItem item)
        {
            return _contextDatabase.DeleteAsync(item);
        }

        public Task DeleteAllMealItemsAsync()
        {
            return _contextDatabase.DeleteAllAsync();
        }
    }
}

