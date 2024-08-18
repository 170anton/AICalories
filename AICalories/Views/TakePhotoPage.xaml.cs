using AICalories.ViewModels;
using Android.Service.Controls.Templates;
using Camera.MAUI;

namespace AICalories.Views;

public partial class TakePhotoPage : ContentPage
{
    private TakePhotoVM _viewModel;

    public TakePhotoPage()
	{
		InitializeComponent();

        _viewModel = new TakePhotoVM();
        BindingContext = _viewModel;

        //var displayInfo = DeviceDisplay.MainDisplayInfo;
        //var screenWidthInDips = displayInfo.Width / displayInfo.Density;
        //var screenHeightInDips = displayInfo.Height / displayInfo.Density;

        cameraView.CamerasLoaded += CameraView_CamerasLoaded;
    }

    private async void OnCaptureButtonClicked(object sender, EventArgs e)
    {
        var stream = await cameraView.TakePhotoAsync();
        if (stream != null)
        {
            var result = ImageSource.FromStream(() => stream);
            cameraView.IsVisible = false;
            captureButton.IsVisible = false;
            snapPreview.Source = result;
        }
        //await Shell.Current.Navigation.PushModalAsync(new LoadingScreenPage());

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
        Shell.Current.GoToAsync("//main");
        return true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

    }
}
