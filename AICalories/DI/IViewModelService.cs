using System;
using AICalories.ViewModels;

namespace AICalories.DI
{
	public interface IViewModelService
	{
        MainVM MainVM { get; set; }
        TakeImageVM TakeImageVM { get; set; }
        ContextVM ContextVM { get; set; }
        ResultVM ResultVM { get; set; }
        AppSettingsVM AppSettingsVM { get; set; }
    }
}

