using System;
using AICalories.ViewModels;

namespace AICalories.DI
{
    public class ViewModelService : IViewModelService
    {
        public PhotoSelectionVM PhotoSelectionVM { get; set; }
        public ContextVM ContextVM { get; set; }
    }
}

