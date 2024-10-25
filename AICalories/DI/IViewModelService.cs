using System;
using AICalories.ViewModels;

namespace AICalories.DI
{
	public interface IViewModelService
	{
        MainVM MainVM { get; set; }
        AppSettingsVM AppSettingsVM { get; set; }
        HistoryVM HistoryVM { get; set; }
        TakeImageVM TakeImageVM { get; set; }
        ContextVM ContextVM { get; set; }
        ResultVM ResultVM { get; set; }
        MealInfoVM MealInfoVM { get; set; }
    }
}

