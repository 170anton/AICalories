using System;
using AICalories.Models;

namespace AICalories.Repositories
{
    public class HistoryItemRepository : IHistoryItemRepository
    {
        private readonly HistoryDatabase<MealItem> _historyDatabase;

        public HistoryItemRepository(HistoryDatabase<MealItem> historyDatabase)
        {
            _historyDatabase = historyDatabase;
        }

        public Task SaveMealItemAsync(MealItem item)
        {
            return _historyDatabase.SaveItemAsync(item);
        }

        public Task<MealItem> GetLastMealItemAsync()
        {
            return _historyDatabase.GetLastItemAsync();
        }

        public Task<List<MealItem>> GetAllMealItemsAsync()
        {
            return _historyDatabase.GetAllItemsAsync();
        }

        public Task<int> GetMealItemsCountAsync()
        {
            return _historyDatabase.GetCountAsync();
        }

        public Task UpdateMealItemAsync(MealItem item)
        {
            return _historyDatabase.UpdateItemAsync(item);
        }

        public Task DeleteMealItemAsync(MealItem item)
        {
            return _historyDatabase.DeleteAsync(item);
        }

        public Task DeleteAllMealItemsAsync()
        {
            return _historyDatabase.DeleteAllAsync();
        }
    }
}

