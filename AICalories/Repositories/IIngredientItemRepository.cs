using AICalories.Models;

namespace AICalories.Repositories
{
    public interface IIngredientItemRepository
    {
        Task<int> DeleteAllIngredientsAsync();
        Task<int> DeleteIngredientAsync(IngredientItem ingredient);
        Task<int> DeleteIngredientByIdAsync(int id);
        Task<List<IngredientItem>> GetAllIngredientsAsync();
        Task<IngredientItem> GetIngredientByIdAsync(int id);
        Task<int> SaveIngredientAsync(IngredientItem ingredient);
    }
}