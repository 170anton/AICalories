using System;
using AICalories.Interfaces;
using AICalories.Models;
using SQLite;

namespace AICalories
{
	public class ContextDatabase<T> where T : class, IContextItem, new()
    {
        private readonly SQLiteAsyncConnection _database;

        public ContextDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<T>().Wait();
        }


        public Task<int> InsertAsync(T item)
        {
            return _database.InsertAsync(item);
        }

        public Task<int> UpdateAsync(T item)
        {
            return _database.UpdateAsync(item);
        }

        public Task<int> DeleteAsync(T item)
        {
            return _database.DeleteAsync(item);
        }

        public Task<List<T>> GetAllItemsAsync()
        {
            return _database.Table<T>().ToListAsync();
        }

        public Task<T> GetItemByIdAsync(int id)
        {
            return _database.Table<T>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<T> GetLastAddedItemAsync()
        {
            return await _database.Table<T>()
                                  .OrderByDescending(item => item.Id)
                                  .FirstOrDefaultAsync();
        }
        //public Task<int> InsertContextItemAsync(T item)
        //{
        //    return _database.InsertAsync(item);
        //}

        //public Task<List<T>> GetAllContextItemsAsync()
        //{
        //    return _database.Table<T>().ToListAsync();
        //}

        //public Task<T> GetContextItemByIdAsync(int id)
        //{
        //    return _database.FindAsync<T>(id);
        //}

        //public Task<int> UpdateContextItemAsync(T item)
        //{
        //    return _database.UpdateAsync(item);
        //}

        //public Task<int> DeleteContextItemAsync(T item)
        //{
        //    return _database.DeleteAsync(item);
        //}

        //public async Task<int> DeleteContextItemByIdAsync(int id)
        //{
        //    var item = await GetContextItemByIdAsync(id);
        //    if (item != null)
        //    {
        //        return await _database.DeleteAsync(item);
        //    }
        //    return 0;
        //}
    }
}

