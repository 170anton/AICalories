using System;
using AICalories.Models;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.Interfaces;

namespace AICalories
{
	public class HistoryDatabase<T> where T : class, IMealItem, new()
    {
        private readonly SQLiteAsyncConnection _database;

        public HistoryDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<T>().Wait();
        }

        public Task<List<T>> GetAllItemsAsync()
        {
            return _database.Table<T>().OrderByDescending(i => i.Date).ToListAsync();
        }

        public Task<T> GetLastItemAsync()
        {
            return _database.Table<T>()
                .OrderByDescending(i => i.Date)
                .FirstOrDefaultAsync();
        }

        public Task<int> GetCountAsync()
        {
            return _database.Table<T>().CountAsync();
        }

        public Task<int> SaveItemAsync(T item)
        {
            if (item.Id != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public Task<int> DeleteAsync(T item)
        {
            return _database.DeleteAsync(item);
        }

        public async Task<int> DeleteAllAsync()
        {
            return await _database.DeleteAllAsync<T>();
        }
    }
}

