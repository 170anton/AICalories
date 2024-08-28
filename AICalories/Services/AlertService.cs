using System;
namespace AICalories.Services
{
    public class AlertService : IAlertService
    {
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

