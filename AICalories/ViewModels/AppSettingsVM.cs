using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.DI;
using AndroidX.Lifecycle;

namespace AICalories.ViewModels
{
    public class AppSettingsVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private const string DarkModeKey = "DarkModeKey";
        private const string SaveToGalleryKey = "SaveToGalleryKey";
        private bool _isDarkMode;
        private bool _isSaveToGallery;

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    OnPropertyChanged();
                    UpdateDarkMode();
                }
            }
        }

        public bool IsSaveToGallery
        {
            get => _isSaveToGallery;
            set
            {
                if (_isSaveToGallery != value)
                {
                    _isSaveToGallery = value;
                    OnPropertyChanged();
                    SaveToGallery();
                }
            }
        }

        public ICommand ToggleDarkModeCommand { get; }
        public ICommand ToggleSaveToGalleryCommand { get; }

        public AppSettingsVM(IViewModelService viewModelService)
        {
            _viewModelService = viewModelService;
            _viewModelService.AppSettingsVM = this;

            IsDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;

            ToggleDarkModeCommand = new Command(() => IsDarkMode = !IsDarkMode);
            ToggleSaveToGalleryCommand = new Command(() => IsSaveToGallery = !IsSaveToGallery);

            LoadSelectedDarkMode();
            LoadSelectedSaveToGallery();
        }

        private void UpdateDarkMode()
        {
            //var theme = IsDarkMode ? "DarkTheme.xaml" : "LightTheme.xaml";
            //Application.Current.Resources.MergedDictionaries.Clear();
            //Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            //{
            //    Source = new Uri($"Resources/{theme}", UriKind.Relative)
            //});
            Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;

            Preferences.Set(DarkModeKey, IsDarkMode);

        }

        private void SaveToGallery()
        {
            //Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;

            Preferences.Set(SaveToGalleryKey, IsSaveToGallery);
        }


        private void LoadSelectedDarkMode()
        {
            var savedOption = Preferences.Get(DarkModeKey, false);
            if (savedOption != null)
            {
                IsDarkMode = savedOption;
            }
        }


        private void LoadSelectedSaveToGallery()
        {
            var savedOption = Preferences.Get(SaveToGalleryKey, false);
            if (savedOption != null)
            {
                IsSaveToGallery = savedOption;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

