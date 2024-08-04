using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.DI;

namespace AICalories.ViewModels
{
	public class ContextVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private string _selectedOption;

        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (_selectedOption != value)
                {
                    _selectedOption = value;
                    OnPropertyChanged();
                }
            }
        }

        public ContextVM(IViewModelService viewModelService)
        {
            _viewModelService = viewModelService;
            _viewModelService.ContextVM = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

