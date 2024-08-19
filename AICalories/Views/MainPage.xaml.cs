using System.Data;
using System.Text;
using AICalories;
using AICalories.DI;
using AICalories.ViewModels;
using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace AICalories.Views;

public partial class MainPage : ContentPage
{
    private MainVM _viewModel;

    #region Constructor

    public MainPage()
	{
		InitializeComponent();
        try
        {
            var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
            if (viewModelLocator != null)
            {
                _viewModel = viewModelLocator.GetMainViewModel();
                BindingContext = _viewModel;
            }
        }
        catch (Exception)
        {
            ShowError("No connection to OpenAI");
        }
        //_viewModel.OnShowResponseRequested += ShowResponse;
        //_viewModel.OnShowAlertRequested += ShowAlert;

    }

    #endregion

    #region FirstFrame

    #endregion

    #region Photo selection buttons

    private async void OnTakeImageClicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            bool isCameraAvailable = await CheckAndRequestCameraPermissionAsync();
            if (isCameraAvailable)
            {
                try
                {
                    //var image = await MediaPicker.Default.CapturePhotoAsync();
                    //await CrossMedia.Current.Initialize();

                    //var takeImagePage = new TakeImagePage();

                    var takeImagePage = IPlatformApplication.Current.Services.GetService<TakeImagePage>();
                    await Shell.Current.Navigation.PushModalAsync(takeImagePage);


                    //await CrossMedia.Current.Initialize();

                    //MediaFile image = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    //{
                    //    PhotoSize = PhotoSize.Medium,
                    //    //SaveToAlbum = true
                    //});

                }
                catch (ArgumentNullException ex)
                {
                    await ShowError("No connection to AI server. Please try again later");
                }
                catch (Exception ex)
                {
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

    //private async void OnSelectImageClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        //var image = await MediaPicker.PickPhotoAsync();
    //        //ProcessImage(image);
    //    }
    //    catch (Exception ex)
    //    {
    //        await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "Sad");
    //    }
    //}

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

    #endregion


    #region View

    private void OnSwipedLeft(object sender, SwipedEventArgs e)
    {
        // Move to the next tab
        //var shell = (AppShell)Application.Current.MainPage;
        //var currentIndex = shell.Items.IndexOf(shell.CurrentItem);
        //if (currentIndex < shell.Items.Count - 1)
        //{
        //    shell.CurrentItem = shell.Items[currentIndex + 1];
        //}
        Shell.Current.GoToAsync("//history");
    }

    private void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        // Move to the previous tab
        //var shell = (AppShell)Application.Current.MainPage;
        //var currentIndex = shell.Items.IndexOf(shell.CurrentItem);
        //if (currentIndex > 0)
        //{
        //    shell.CurrentItem = shell.Items[currentIndex - 1];
        //}

        Shell.Current.GoToAsync("//settings");
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


    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //_viewModel.OnShowResponseRequested -= ShowResponse;
        //_viewModel.OnShowAlertRequested -= ShowAlert;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadLastHistoryItem();
    }
    #endregion

}