using System;
using AICalories.Interfaces;
using SQLite;

namespace AICalories
{
	public class IngredientsDatabase<T> where T : class, IIngredientItem, new()
    {
        private readonly SQLiteAsyncConnection _database;

        public IngredientsDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<T>().Wait();
        }

        // Create or Update an ingredient
        public async Task<int> SaveIngredientAsync(T ingredient)
        {
            if (ingredient.Id != 0)
            {
                return await _database.UpdateAsync(ingredient);
            }
            else
            {
                return await _database.InsertAsync(ingredient);
            }
        }

        public async Task<T> GetIngredientAsync(int id)
        {
            return await _database.Table<T>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetIngredientsAsync()
        {
            return await _database.Table<T>().ToListAsync();
        }

        public async Task<int> DeleteIngredientAsync(T ingredient)
        {
            return await _database.DeleteAsync(ingredient);
        }

        public async Task<int> DeleteAllIngredientsAsync()
        {
            return await _database.DeleteAllAsync<T>();
        }

    }
}

