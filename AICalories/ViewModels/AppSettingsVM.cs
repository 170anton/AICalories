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
        //private const string DarkModeKey = "DarkModeKey";
        //private const string SaveToGalleryKey = "SaveToGalleryKey";
        private bool _isDarkMode;
        private bool _isShowReview;
        private bool _isSaveToGallery;

        private string _email;
        private string _question;
        private bool _isLoading;
        private bool _isLayoutVisible;
        private const int MaxQuestionsPerHour = 2;
        private const string QuestionTimestampsKey = "QuestionTimestamps";

        #region Properties

        public string AppVersion { get; set; }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    UpdateDarkMode();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsShowReview
        {
            get => _isShowReview;
            set
            {
                if (_isShowReview != value)
                {
                    _isShowReview = value;
                    UpdateShowReview();
                    OnPropertyChanged();
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
                    UpdateSaveToGallery();
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Question
        {
            get => _question;
            set
            {
                _question = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsLayoutVisible
        {
            get => _isLayoutVisible;
            set
            {
                _isLayoutVisible = value;
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

            AppVersion = $"Version: {AppInfo.VersionString}";

            //IsDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;

            ToggleDarkModeCommand = new Command(() => IsDarkMode = !IsDarkMode);
            ToggleSaveToGalleryCommand = new Command(() => IsSaveToGallery = !IsSaveToGallery);
            SendQuestionCommand = new Command(async () => await Send());

            LoadSelectedDarkMode();
            LoadSelectedShowReview();
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

            Preferences.Set(App.DarkModeKey, IsDarkMode);
            //Preferences.Set(App.SaveToGalleryKey, IsSaveToGallery);

        }

        private void UpdateSaveToGallery()
        {
            //Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;

            Preferences.Set(App.SaveToGalleryKey, IsSaveToGallery);
        }

        private void UpdateShowReview()
        {
            //Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;

            Preferences.Set(App.ShowReviewKey, IsShowReview);
        }
        

        private void LoadSelectedDarkMode()
        {
            var savedOption = Preferences.Get(App.DarkModeKey, false);
            IsDarkMode = savedOption;
        }

        private void LoadSelectedShowReview()
        {
            var savedOption = Preferences.Get(App.ShowReviewKey, true);
            IsShowReview = savedOption;
        }


        private void LoadSelectedSaveToGallery()
        {
            var savedOption = Preferences.Get(App.SaveToGalleryKey, false);
            IsSaveToGallery = savedOption;
        }
        #endregion

        #region Support

        private async Task Send()
        {
            try
            {
                if (!await CheckPrerequirements())
                {
                    return;
                }

                IsLoading = true;
                var success = await EmailService.SendEmailAsync(Email, Question, AppVersion);
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
                Console.WriteLine($"Error sending email: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred. Please try again", "Sad");
            }
            finally
            {
                IsLoading = false; // Re-enable the button
            }
        }

        private async Task<bool> CheckPrerequirements()
        {

            if (!InternetConnection.CheckInternetConnection())
            {
                DisplayAlertConfiguration.ShowError("No internet connection");
                return false;
            }

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Question))
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Please complete all fields", "OK");
                return false;
            }

            if (Email.Length <= 5)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Your email is too short", "OK");
                return false;
            }

            if (Question.Length <= 10)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Your question is too short", "OK");
                return false;
            }

            if (HasExceededQuestionLimit())
            {
                await Application.Current.MainPage.DisplayAlert("Too many requests", "Please try again later", "OK");
                return false;
            }

            return true;
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


        public async Task OnPageAppearingAsync()
        {
            try
            {
                IsLoading = true;
                //await Task.Delay(500);
                IsLayoutVisible = true;
                IsLoading = false;
            }
            catch (Exception)
            {
                DisplayAlertConfiguration.ShowError("Loading error occurred");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

