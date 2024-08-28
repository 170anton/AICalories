using System;
using AICalories.ViewModels;

namespace AICalories.DI
{
    public class ViewModelService : IViewModelService
    {
        public MainVM MainVM { get; set; }
        public TakeImageVM TakeImageVM { get; set; }
        public ContextVM ContextVM { get; set; }
        public ResultVM ResultVM { get; set; }
        public AppSettingsVM AppSettingsVM { get; set; }
    }
}

