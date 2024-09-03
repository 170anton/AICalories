using System;
namespace AICalories.Services
{
    public interface INavigationService
    {
        Task PushModalAsync(Page page);
        Task PopModalAsync();
        Task PopToMainModalAsync();
        Task NavigateToMainPageAsync();
        Task NavigateToTakeImagePageAsync();
        Task NavigateToContextPageAsync();
        Task NavigateToResultPageAsync();
    }
}

