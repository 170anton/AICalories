using AICalories.Interfaces;
using AICalories.Models;
using AICalories.ViewModels;
using Camera.MAUI;
using Plugin.Media.Abstractions;

namespace AICalories.Views;

public partial class TakeImagePage : ContentPage
{
    private TakeImageVM _viewModel;

    public TakeImagePage(TakeImageVM viewModel)
	{
		InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;

        //var displayInfo = DeviceDisplay.MainDisplayInfo;
        //var screenWidthInDips = displayInfo.Width / displayInfo.Density;
        //var screenHeightInDips = displayInfo.Height / displayInfo.Density;

        cameraView.CamerasLoaded += CameraView_CamerasLoaded;
    }

    private async void OnCaptureButtonClicked(object sender, EventArgs e)
    {
        var stream = await cameraView.TakePhotoAsync();

        if (stream == null)
        {
            return; //todo
        }

        var imageName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
        var imagePath = Path.Combine(FileSystem.CacheDirectory, imageName);

        using (var fileStream = File.Create(imagePath))
        {
            await stream.CopyToAsync(fileStream);
        }

        //var image = ImageSource.FromStream(() => stream);


        _viewModel.SetImage(imagePath);


        Shell.Current.Navigation.PopModalAsync();
        await Shell.Current.Navigation.PushModalAsync(new ContextPage());


    }

    private async void OnGalleryButtonClicked(object sender, EventArgs e)
    {
        var image = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Please select an image"
        });


        if (image != null)
        {
            return; //todo
        }

        _viewModel.SetImage(image.FullPath);
        ContextPage contextPage = new ContextPage();
        await Shell.Current.Navigation.PushModalAsync(contextPage);
        

    }

    private void ToggleVisibility()
    {
        cameraView.IsVisible = !cameraView.IsVisible;
        captureButton.IsVisible = !captureButton.IsVisible;
        galleryButton.IsVisible = !galleryButton.IsVisible;
        imagePreview.IsVisible = !imagePreview.IsVisible;
    }

    private void CameraView_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.NumCamerasDetected > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();


            cameraView.StartCameraAsync(new Size(1440, 1080)); // todo make mode resolutions if unavailable 0.75
            //cameraView.StartCameraAsync(cameraView.Camera.AvailableResolutions.OrderByDescending
            //            (size => size.Width * size.Height).FirstOrDefault());
            cameraView.ForceAutoFocus();
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (imagePreview.IsVisible)
        {
            ToggleVisibility();
            return true;
        }

        Shell.Current.Navigation.PopModalAsync();
        //Shell.Current.GoToAsync("//main");
        return true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

    }
}
