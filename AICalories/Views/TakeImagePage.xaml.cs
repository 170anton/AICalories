using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Services;
using AICalories.ViewModels;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Camera;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Core;

namespace AICalories.Views;

public partial class TakeImagePage : ContentPage
{
    private TakeImageVM _viewModel;

    public TakeImagePage()
	{
		InitializeComponent();

        var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
        if (viewModelLocator == null)
        {
            return;
        }

        //var cameraService = new CameraService(cameraView);

        _viewModel = viewModelLocator.GetTakeImageViewModel(cameraView);
        BindingContext = _viewModel;

        cameraView.Loaded += CameraView_CamerasLoaded;
        //cameraView.MediaCaptured += CameraView_Cuptured;
    }

    private async void CaptureButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            await cameraView.CaptureImage(CancellationToken.None);
        }
        catch (Exception ex)
        {

        }
    }

    private async void CameraView_Captured(object? sender, MediaCapturedEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () => //todo
        {
            try
            {
                var imageName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                var imagePath = Path.Combine(FileSystem.CacheDirectory, imageName);

                await using (var localFileStream = File.Create(imagePath))
                {
                    e.Media.CopyTo(localFileStream);
                }
                cameraView.StopCameraPreview();
                _viewModel.CaptureCommand.Execute(imagePath);
            }
            catch (Exception ex)
            {
                //todo
            }
        });
    }

    //private async void OnCaptureButtonClicked(object sender, EventArgs e)
    //{
    //    if (cameraView.TorchEnabled)
    //    {
    //        cameraView.FlashMode = FlashMode.Enabled;
    //    }
    //    var stream = await cameraView.TakePhotoAsync();

    //    if (stream == null)
    //    {
    //        return; //todo
    //    }

    //    var imageName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
    //    var imagePath = Path.Combine(FileSystem.CacheDirectory, imageName);

    //    await using (var fileStream = File.Create(imagePath))
    //    {
    //        await stream.CopyToAsync(fileStream);
    //    }

    //    //var image = ImageSource.FromStream(() => stream);

    //    _viewModel.SetImage(imagePath);

    //    Shell.Current.Navigation.PopModalAsync();
    //    await Shell.Current.Navigation.PushModalAsync(new ContextPage());


    //}

    //private async void OnGalleryButtonClicked(object sender, EventArgs e)
    //{
    //    var image = await FilePicker.PickAsync(new PickOptions
    //    {
    //        FileTypes = FilePickerFileType.Images,
    //        PickerTitle = "Please select an image"
    //    });

    //    //todo copy to local rep like on capture
    //    if (image == null)
    //    {
    //        return; //todo
    //    }

    //    _viewModel.SetImage(image.FullPath);

    //    Shell.Current.Navigation.PopModalAsync();
    //    await Shell.Current.Navigation.PushModalAsync(new ContextPage());


    //}
    //private async void OnToggleTorchButtonClicked(object sender, EventArgs e)
    //{
    //    cameraView.TorchEnabled = !cameraView.TorchEnabled;
    //}

    private void ToggleVisibility()
    {
        cameraView.IsVisible = !cameraView.IsVisible;
        captureButton.IsVisible = !captureButton.IsVisible;
        galleryButton.IsVisible = !galleryButton.IsVisible;
    }

    private void CameraView_CamerasLoaded(object sender, EventArgs e)
    {

        cameraView.ImageCaptureResolution = new Size(1440, 1080);


    }

    protected override bool OnBackButtonPressed()
    {

        Shell.Current.Navigation.PopModalAsync();
        //Shell.Current.GoToAsync("//main");
        return true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        //if (cameraView.TorchEnabled)
        //{
        //    cameraView.TorchEnabled = false;
        //}
        //cameraView.StopCameraAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

    }
}
