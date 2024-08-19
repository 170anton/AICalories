using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Interfaces;

namespace AICalories.ViewModels
{
	public class ContextVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private const string SelectedOptionKey = "SelectedOption";

        private string _selectedOption;
        private string _mainImage;
        private IImageInfo _imageInfo;

        #region Properties

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

        public string MainImage
        {
            get => _imageInfo.ImagePath;
            set
            {
                if (_imageInfo.ImagePath != value)
                {
                    _imageInfo.ImagePath = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        public ICommand ConfirmCommand { get; }

        public ContextVM(IViewModelService viewModelService, IImageInfo imageInfo)
        {
            _viewModelService = viewModelService;
            _viewModelService.ContextVM = this;

            _imageInfo = imageInfo;


            LoadSelectedOption();
        }

        public async void SetAdditionalInfo()
        {
            _imageInfo.MealType = SelectedOption;
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

