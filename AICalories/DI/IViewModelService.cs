using System;
using AICalories.ViewModels;

namespace AICalories.DI
{
	public interface IViewModelService
	{
        PhotoSelectionVM PhotoSelectionVM { get; set; }
        ContextVM ContextVM { get; set; }
        AppSettingsVM AppSettingsVM { get; set; }
    }
}

