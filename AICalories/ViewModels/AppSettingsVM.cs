using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AICalories.ViewModels
{
    public class AppSettingsVM : INotifyPropertyChanged
    {
        private bool _isDarkMode;

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    OnPropertyChanged();
                    UpdateTheme();
                }
            }
        }

        public ICommand ToggleThemeCommand { get; }

        public AppSettingsVM()
		{
            IsDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;

            ToggleThemeCommand = new Command(() => IsDarkMode = !IsDarkMode);
        }

        private void UpdateTheme()
        {
            //var theme = IsDarkMode ? "DarkTheme.xaml" : "LightTheme.xaml";
            //Application.Current.Resources.MergedDictionaries.Clear();
            //Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            //{
            //    Source = new Uri($"Resources/{theme}", UriKind.Relative)
            //});
            Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

