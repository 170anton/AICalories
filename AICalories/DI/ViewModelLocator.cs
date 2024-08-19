using System;
using AICalories.Interfaces;
using AICalories.ViewModels;

namespace AICalories.DI
{
	public class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IImageInfo _imageInfo;

        public ViewModelLocator(IServiceProvider serviceProvider, IImageInfo imageInfo)
        {
            _serviceProvider = serviceProvider;
            _imageInfo = imageInfo;
        }

        public MainVM GetMainViewModel() => _serviceProvider.GetRequiredService<MainVM>();
        public ContextVM GetContextViewModel() => _serviceProvider.GetRequiredService<ContextVM>();
        public AppSettingsVM GetAppSettingsViewModel() => _serviceProvider.GetRequiredService<AppSettingsVM>();

    }
}

