using System;
using AICalories.Interfaces;
using AICalories.Models;

namespace AICalories.Repositories
{
	public interface IContextItemRepository
	{
        Task UpdateContextItemAsync(ContextItem item);
        Task<ContextItem> GetContextItemByIdAsync(int id);
        Task<List<ContextItem>> GetAllContextItemsAsync();
        Task DeleteContextItemAsync(ContextItem item);
        Task<ContextItem> GetLastAddedContextItemAsync();
        Task<int> InsertContextItemAsync(ContextItem item);
    }
}

