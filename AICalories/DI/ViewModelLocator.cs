using System;
using AICalories.ViewModels;

namespace AICalories.DI
{
	public class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public PhotoSelectionVM GetPhotoSelectionViewModel() => _serviceProvider.GetRequiredService<PhotoSelectionVM>();
        public ContextVM GetContextViewModel() => _serviceProvider.GetRequiredService<ContextVM>();
        public AppSettingsVM GetAppSettingsViewModel() => _serviceProvider.GetRequiredService<AppSettingsVM>();

    }
}

