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

        public async Task<List<T>> GetAllItemsByMonthAsync(DateTime dateTime)
        {
            var items = await _database.Table<T>().ToListAsync();
            return items
                .Where(item => item.Date.Month == dateTime.Month && item.Date.Year == dateTime.Year)
                .OrderByDescending(i => i.Date)
                .ToList();
        }

        public async Task<T> GetOldestItemAsync()
        {
            return await _database.Table<T>()
                .OrderBy(item => item.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<T> GetLastItemAsync()
        {
            return await _database.Table<T>()
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

        public Task<int> UpdateItemAsync(T item)
        {
            return _database.UpdateAsync(item);
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

