using System.Data;
using System.Text;
using AICalories;
using AICalories.ViewModels;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace AICalories.Views;

public partial class PhotoSelectionPage : ContentPage
{
    private string _photoPath;

    private PhotoSelectionVM _viewModel;

    public PhotoSelectionPage()
	{
		InitializeComponent();

        _viewModel = new PhotoSelectionVM();
        BindingContext = _viewModel;
        _viewModel.OnShowResponseRequested += ShowResponse;
        _viewModel.OnShowAlertRequested += ShowAlert;

    }

    private async void OnTakeImageClicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            bool isCameraAvailable = await CheckAndRequestCameraPermissionAsync();
            if (isCameraAvailable)
            {
                try
                {
                    var image = await MediaPicker.CapturePhotoAsync();

                    if (image != null)
                    {
                        //App.Current.MainPage = new NavigationPage(new LoadScreenPage());
                        _viewModel.ProcessImage(image);//await
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors that occurred during photo capture
                    await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Permission Denied", "Camera permission is required to take photos.", "OK");
            }
            //FileResult? image;
            //var device = DeviceInfo.Platform;
            //var ead = DevicePlatform.macOS;
            //if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
            //{
            //    image = await MediaPicker.CapturePhotoAsync();
            //}
            //else
            //{
            //    image = await MediaPicker.CapturePhotoAsync();
            //}
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "Sad");
        }
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        try
        {
            var image = await MediaPicker.PickPhotoAsync();

            if (image != null)
            {
                _viewModel.ProcessImage(image);//await
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "Sad");
        }
    }

    private async Task ShowResponse(string response)
    {
        try
        {
            await Application.Current.MainPage.DisplayAlert("Analysis Result", response, "Ok");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "Sad");
        }
    }

    private async Task ShowAlert(string response)
    {
        try
        {
            await Application.Current.MainPage.DisplayAlert("Alert", response, "Sad");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "Sad");
        }
    }

    private async Task<bool> CheckAndRequestCameraPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            // Handle the exception as needed
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            return false;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnShowResponseRequested -= ShowResponse;
        _viewModel.OnShowAlertRequested -= ShowAlert;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        //if (_viewModel.HasRecievedSecrets == false)
        //{
        //    string message = "No connection to OpenAI"; //todo
        //    ShowAlert(message);
        //}
    }
}