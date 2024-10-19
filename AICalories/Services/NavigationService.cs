using System;
using AICalories.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AICalories.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task PushModalAsync(Page page)
        {
            return Shell.Current.Navigation.PushModalAsync(page);
        }

        public Task PopModalAsync()
        {
            return Shell.Current.Navigation.PopModalAsync();
        }

        public Task PopToMainModalAsync()
        {
            return Shell.Current.Navigation.PopToRootAsync();
        }

        public Task NavigateToMainPageAsync()
        {
            var resultPage = _serviceProvider.GetService<MainPage>();
            return Shell.Current.Navigation.PushModalAsync(resultPage);
        }

        public Task NavigateToTakeImagePageAsync()
        {
            var resultPage = _serviceProvider.GetService<TakeImagePage>();
            return Shell.Current.Navigation.PushModalAsync(resultPage);
        }

        public Task NavigateToTakeImagePageAsyncNotModal()
        {
            var resultPage = _serviceProvider.GetService<TakeImagePage>();
            return Shell.Current.Navigation.PushAsync(resultPage);
        }

        public Task NavigateToContextPageAsync()
        {
            var resultPage = _serviceProvider.GetService<ContextPage>();
            return Shell.Current.Navigation.PushModalAsync(resultPage);
        }

        public Task NavigateToResultPageAsync()
        {
            var resultPage = _serviceProvider.GetService<ResultPage>();
            return Shell.Current.Navigation.PushModalAsync(resultPage);
        }
    }
}

