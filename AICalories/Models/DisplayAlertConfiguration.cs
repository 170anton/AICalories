using System;
namespace AICalories.Models
{
	public static class DisplayAlertConfiguration
    {

        public static async void ShowError(string error)
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert("Error", error, "Sad");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An unexpected error occurred", "Sad");
            }
        }

        public static async void ShowUnexpectedError()
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"An unexpected error occurred", "Sad");
        }
    }
}

