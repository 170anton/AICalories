using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.Models;

namespace AICalories.ViewModels
{
    public class SupportVM : INotifyPropertyChanged
    {
        private string email;
        private string question;
        private bool isSending;
        private const int MaxQuestionsPerHour = 2;
        private const string QuestionTimestampsKey = "QuestionTimestamps";

        public ICommand SendCommand { get; }

        public SupportVM()
        {
            SendCommand = new Command(async () => await Send());
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

        public bool IsSending
        {
            get => isSending;
            set
            {
                isSending = value;
                OnPropertyChanged();
            }
        }


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
                IsSending = true; 
                var success = await EmailService.SendEmailAsync(Email, Question);

                if (success)
                {
                    IsSending = false;
                    Email = null;
                    Question = null;
                    AddTimestamp();
                    await Application.Current.MainPage.DisplayAlert("Success", "Your question has been sent", "OK");
                }
                else
                {
                    IsSending = false;
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
                IsSending = false; // Re-enable the button
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
