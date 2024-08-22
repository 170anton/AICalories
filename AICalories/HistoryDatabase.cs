using System;
using AICalories.Models;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AICalories
{
	public class HistoryDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public HistoryDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<HistoryItem>().Wait();
            //_database.CreateTableAsync<HistorySubItem>().Wait();
        }

        public async Task<List<HistoryItem>> GetItemsAsync()
        {
            return await _database.Table<HistoryItem>().ToListAsync();
        }

        public async Task<HistoryItem> GetLastItemAsync()
        {
            return await _database.Table<HistoryItem>()
                .OrderByDescending(i => i.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _database.Table<HistoryItem>().CountAsync();
        }

        public async Task<int> SaveItemAsync(HistoryItem item)
        {
            if (item.Id != 0)
            {
                return await _database.UpdateAsync(item);
            }
            else
            {
                return await _database.InsertAsync(item);
            }
        }

        public async Task<int> DeleteItemAsync(HistoryItem item)
        {
            return await _database.DeleteAsync(item);
        }

        public async Task<int> ClearItemsAsync()
        {
            return await _database.DeleteAllAsync<HistoryItem>();
        }

        //public Task<List<HistorySubItem>> GetSubItemsAsync(int itemId)
        //{
        //    return _database.Table<HistorySubItem>().Where(si => si.ItemId == itemId).ToListAsync();
        //}

        //public Task<int> SaveSubItemAsync(HistorySubItem subItem)
        //{
        //    if (subItem.Id != 0)
        //    {
        //        return _database.UpdateAsync(subItem);
        //    }
        //    else
        //    {
        //        return _database.InsertAsync(subItem);
        //    }
        //}

        //public Task<int> DeleteSubItemAsync(HistorySubItem subItem)
        //{
        //    return _database.DeleteAsync(subItem);
        //}

        //public Task<int> ClearSubItemsAsync()
        //{
        //    return _database.DeleteAllAsync<HistorySubItem>();
        //}
    }
}

