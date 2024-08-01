using System;
using AICalories.Models;
using SQLite;

namespace AICalories
{
	public class DatabaseHelper
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<HistoryItem>().Wait();
            //_database.CreateTableAsync<HistorySubItem>().Wait();
        }

        public Task<List<HistoryItem>> GetItemsAsync()
        {
            return _database.Table<HistoryItem>().ToListAsync();
        }

        public Task<int> SaveItemAsync(HistoryItem item)
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

        public Task<int> DeleteItemAsync(HistoryItem item)
        {
            return _database.DeleteAsync(item);
        }

        public Task<int> ClearItemsAsync()
        {
            return _database.DeleteAllAsync<HistoryItem>();
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

