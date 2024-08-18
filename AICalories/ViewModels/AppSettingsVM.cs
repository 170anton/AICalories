using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Models;

namespace AICalories.ViewModels
{
    public class AppSettingsVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private const string DarkModeKey = "DarkModeKey";
        private const string SaveToGalleryKey = "SaveToGalleryKey";
        private bool _isDarkMode;
        private bool _isSaveToGallery;

        private string email;
        private string question;
        private bool isLoading;
        private const int MaxQuestionsPerHour = 2;
        private const string QuestionTimestampsKey = "QuestionTimestamps";

        #region Properties

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

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        public string Question
        {
            get => question;
            set
            {
                question = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ICommand ToggleDarkModeCommand { get; }
        public ICommand ToggleSaveToGalleryCommand { get; }
        public ICommand SendQuestionCommand { get; }

        #region Constructor

        public AppSettingsVM(IViewModelService viewModelService)
        {
            _viewModelService = viewModelService;
            _viewModelService.AppSettingsVM = this;

            //IsDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;

            ToggleDarkModeCommand = new Command(() => IsDarkMode = !IsDarkMode);
            ToggleSaveToGalleryCommand = new Command(() => IsSaveToGallery = !IsSaveToGallery);
            SendQuestionCommand = new Command(async () => await Send());

            LoadSelectedDarkMode();
            LoadSelectedSaveToGallery();
        }
        #endregion

        #region Settings

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
        #endregion

        #region Support

        private async Task Send()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Question))
            {
                // Display alert if fields are empty
                await Application.Current.MainPage.DisplayAlert("Warning", "Please, fill in all fields", "OK");
                return;
            }


            if (HasExceededQuestionLimit())
            {
                await Application.Current.MainPage.DisplayAlert("Too many requests", "Please try again later", "OK");
                return;
            }


            try
            {
                IsLoading = true;
                var success = await EmailService.SendEmailAsync(Email, Question);
                IsLoading = false;

                if (success)
                {
                    Email = null;
                    Question = null;
                    AddTimestamp();
                    await Application.Current.MainPage.DisplayAlert("Success", "Your question has been sent", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to send your question. Try again later", "Sad");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"Error sending email: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred. Please try again", "Sad");
            }
            finally
            {
                IsLoading = false; // Re-enable the button
            }
        }

        private bool HasExceededQuestionLimit()
        {
            var timestamps = GetTimestamps();
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);

            // Remove timestamps that are older than one hour
            timestamps = timestamps.Where(ts => ts > oneHourAgo).ToList();

            // Update the stored timestamps
            Preferences.Set(QuestionTimestampsKey, string.Join(",", timestamps));

            return timestamps.Count >= MaxQuestionsPerHour;
        }

        private void AddTimestamp()
        {
            var timestamps = GetTimestamps();
            timestamps.Add(DateTime.UtcNow);

            // Update the stored timestamps
            Preferences.Set(QuestionTimestampsKey, string.Join(",", timestamps));
        }


        private List<DateTime> GetTimestamps()
        {
            var timestamps = Preferences.Get(QuestionTimestampsKey, string.Empty);

            return string.IsNullOrEmpty(timestamps)
                ? new List<DateTime>()
                : timestamps.Split(',').Select(ts => DateTime.Parse(ts)).ToList();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

