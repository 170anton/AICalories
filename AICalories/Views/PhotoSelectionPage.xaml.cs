using System.Data;
using System.Text;
using AICalories;
using AICalories.ViewModels;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json;

namespace AICalories.Views;

public partial class PhotoSelectionPage : ContentPage
{
    private PhotoSelectionVM _viewModel;
    private LoadingScreenPage _loadScreenPage;

    public PhotoSelectionPage()
	{
		InitializeComponent();
        try
        {
            _viewModel = new PhotoSelectionVM();
            BindingContext = _viewModel;
        }
        catch (Exception)
        {
            ShowError("No connection to OpenAI");
        }
        //_viewModel.OnShowResponseRequested += ShowResponse;
        //_viewModel.OnShowAlertRequested += ShowAlert;

    }

    private async Task ProcessImage(FileResult? image)
    {
        if (image != null)
        {
            _loadScreenPage = new LoadingScreenPage();
            await Shell.Current.Navigation.PushAsync(_loadScreenPage);
            var response = await _viewModel.ProcessImage(image);
            if (response != null)
            {
                _loadScreenPage.LoadAIResponse(response);
            }
            else
            {
                _loadScreenPage.LoadAIResponse("Loading error");
            }
        }
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
                    await ProcessImage(image);
                }
                catch (ArgumentNullException ex)
                {
                    _loadScreenPage.LoadAIResponse("Loading error");
                    await ShowError("No connection to OpenAI");
                }
                catch (Exception ex)
                {
                    _loadScreenPage.LoadAIResponse("Loading error");
                    await ShowError($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Permission Denied", "Camera permission is required to take photos.", "OK");
            }
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
            await ProcessImage(image);
        }
        catch (Exception ex)
        {
            _loadScreenPage.LoadAIResponse("Loading error");
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

    private async Task ShowError(string response)
    {
        try
        {
            await Application.Current.MainPage.DisplayAlert("Error", response, "Sad");
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
        //_viewModel.OnShowResponseRequested -= ShowResponse;
        //_viewModel.OnShowAlertRequested -= ShowAlert;
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