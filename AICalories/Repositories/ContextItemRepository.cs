using System;
using AICalories.Models;

namespace AICalories.Repositories
{
	public class ContextItemRepository : IContextItemRepository
    {
        private readonly ContextDatabase<ContextItem> _contextDatabase;

        public ContextItemRepository(ContextDatabase<ContextItem> contextDatabase)
        {
            _contextDatabase = contextDatabase;
        }

        public Task UpdateContextItemAsync(ContextItem item)
        {
            return _contextDatabase.UpdateAsync(item);
        }

        public Task<ContextItem> GetContextItemByIdAsync(int id)
        {
            return _contextDatabase.GetItemByIdAsync(id);
        }

        public Task<List<ContextItem>> GetAllContextItemsAsync()
        {
            return _contextDatabase.GetAllItemsAsync();
        }

        public Task DeleteContextItemAsync(ContextItem item)
        {
            return _contextDatabase.DeleteAsync(item);
        }

        public Task<ContextItem> GetLastAddedContextItemAsync()
        {
            return _contextDatabase.GetLastAddedItemAsync();
        }

        public Task<int> InsertContextItemAsync(ContextItem item)
        {
            return _contextDatabase.InsertAsync(item);
        }
    }
}

