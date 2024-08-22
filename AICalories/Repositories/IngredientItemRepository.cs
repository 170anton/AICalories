using System;
using AICalories.Models;

namespace AICalories.Repositories
{
    public class IngredientItemRepository : IIngredientItemRepository
    {
        private readonly IngredientsDatabase<IngredientItem> _ingredientsDatabase;

        public IngredientItemRepository(IngredientsDatabase<IngredientItem> ingredientsDatabase)
        {
            _ingredientsDatabase = ingredientsDatabase;
        }

        public async Task<int> SaveIngredientAsync(IngredientItem ingredient)
        {
            return await _ingredientsDatabase.SaveIngredientAsync(ingredient);
        }

        public async Task<IngredientItem> GetIngredientByIdAsync(int id)
        {
            return await _ingredientsDatabase.GetIngredientAsync(id);
        }

        public async Task<List<IngredientItem>> GetAllIngredientsAsync()
        {
            return await _ingredientsDatabase.GetIngredientsAsync();
        }

        public async Task<int> DeleteIngredientAsync(IngredientItem ingredient)
        {
            return await _ingredientsDatabase.DeleteIngredientAsync(ingredient);
        }

        public async Task<int> DeleteIngredientByIdAsync(int id)
        {
            var ingredient = await GetIngredientByIdAsync(id);
            if (ingredient != null)
            {
                return await DeleteIngredientAsync(ingredient);
            }
            return 0;
        }

        public async Task<int> DeleteAllIngredientsAsync()
        {
            return await _ingredientsDatabase.DeleteAllIngredientsAsync();
        }
    }
}

