﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.DI;
using AndroidX.Lifecycle;

namespace AICalories.ViewModels
{
	public class ContextVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private const string SelectedOptionKey = "SelectedOption";

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

        //public ICommand SetSelectedOptionCommand { get; }

        public ContextVM(IViewModelService viewModelService)
        {
            _viewModelService = viewModelService;
            _viewModelService.ContextVM = this;
            //SetSelectedOptionCommand = new Command(OnSetSelectedOption);

            LoadSelectedOption();
        }

        private void LoadSelectedOption()
        {
            var savedOption = Preferences.Get(SelectedOptionKey, "Regular");
            if (savedOption != null)
            {
                SelectedOption = savedOption;
            }
        }

        //private void OnSetSelectedOption(object sender, CheckedChangedEventArgs e)
        //{
        //    if (e.Value)
        //    {
        //        var radioButton = sender as RadioButton;
        //        var selectedOption = radioButton?.Value?.ToString();
        //        // Save the selected option to preferences
        //        Preferences.Set(SelectedOptionKey, selectedOption);

        //        SelectedOption = selectedOption;
        //    }
        //}


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

