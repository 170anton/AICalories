using System;
namespace AICalories.Services
{
    public class AlertService : IAlertService
    {
        public void ShowCustomAlert(string title, string message)
        {
            Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        public void ShowError(string message)
        {
            Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }

        public void ShowUnexpectedError()
        {
            Application.Current.MainPage.DisplayAlert("Error", $"An unexpected error occurred", "Sad");
        }
    }
}

