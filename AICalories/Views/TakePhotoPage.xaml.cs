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
        await Shell.Current.Navigation.PushModalAsync(new LoadingScreenPage());

    }

    private void CameraView_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.NumCamerasDetected > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();
            cameraView.StartCameraAsync(new Size(1440, 1080)); // todo make mode resolutions if unavailable 0.75
            //cameraView.ZoomFactor = 2.5f;
            //MainThread.BeginInvokeOnMainThread(async () =>
            //{
            //    await cameraView.StartCameraAsync(new Size(1920, 1080));
            //});
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
