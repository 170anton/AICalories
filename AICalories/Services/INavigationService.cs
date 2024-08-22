using System;
namespace AICalories.Services
{
    public interface INavigationService
    {
        Task PushModalAsync(Page page);
        Task PopModalAsync();
        Task NavigateToMainPageAsync();
        Task NavigateToTakeImagePageAsync();
        Task NavigateToContextPageAsync();
        Task NavigateToResultPageAsync();
    }
}

