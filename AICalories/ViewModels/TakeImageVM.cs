using System;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Services;
using Android.Content;
using Android.Provider;

namespace AICalories.ViewModels
{
	public class TakeImageVM
    {
        private readonly IViewModelService _viewModelService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        private readonly ICameraService _cameraService;

        private IImageInfo _imageInfo;

        public ICommand CaptureCommand { get; }
        public ICommand GalleryCommand { get; }
        public ICommand ToggleTorchCommand { get; }


        public TakeImageVM(IViewModelService viewModelService, IImageInfo imageInfo, ICameraService cameraService,
            INavigationService navigationService, IAlertService alertService)
        {
            _viewModelService = viewModelService;
            _viewModelService.TakeImageVM = this;
            _navigationService = navigationService;
            _alertService = alertService;
            _cameraService = cameraService;

            _imageInfo = imageInfo;
            _imageInfo.Clear();

            CaptureCommand = new Command(async () => await OnCaptureButtonClicked());
            GalleryCommand = new Command(async () => await OnGalleryButtonClicked());
            ToggleTorchCommand = new Command(OnToggleTorchButtonClicked);
        }


        public async void SetImage(string imagePath)
        {
            _imageInfo.ImagePath = imagePath;
        }

        private async Task OnCaptureButtonClicked() //todo add try catch
        {
            try
            {
                var stream = await _cameraService.TakePhotoAsync();

                if (stream == null)
                {
                    return;
                }

                string imagePath = await SaveImage(stream);

                SaveToGallery(imagePath);

                SetImage(imagePath);

                await CheckShowReviewKey();
            }
            catch (Exception)
            {
                _navigationService.PopModalAsync();
                _alertService.ShowError("Failed to load image");
            }
        }

        private async Task OnGalleryButtonClicked()
        {
            try
            {

                var image = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Please select an image"
                });

                if (image == null)
                {
                    return;
                }

                var imageName = $"gallery_image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                var imagePath = Path.Combine(FileSystem.CacheDirectory, imageName);

                using (var fileStream = File.Create(imagePath))
                {
                    using (var pickedStream = await image.OpenReadAsync())
                    {
                        await pickedStream.CopyToAsync(fileStream);
                    }
                }

                SetImage(imagePath);

                await CheckShowReviewKey();
            }
            catch (Exception)
            {
                _navigationService.PopModalAsync();
                _alertService.ShowError("Failed to load image");
            }

        }


        private void OnToggleTorchButtonClicked()
        {
            if (_cameraService.IsTorchEnabled())
            {
                _cameraService.DisableTorch();
            }
            else
            {
                _cameraService.EnableTorch();
            }
        }


        private async Task<string> SaveImage(Stream stream)
        {
            var imageName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            var imagePath = Path.Combine(FileSystem.CacheDirectory, imageName);

            await using (var fileStream = File.Create(imagePath))
            {
                await stream.CopyToAsync(fileStream);
            }

            return imagePath;
        }

        private void SaveToGallery(string imagePath)
        {
            if (Preferences.Get(App.SaveToGalleryKey, false))
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    var fileName = Path.GetFileName(imagePath);
                    var mimeType = "image/jpeg";
                    var values = new ContentValues();
                    values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
                    values.Put(MediaStore.IMediaColumns.MimeType, mimeType);
                    values.Put(MediaStore.Images.ImageColumns.RelativePath, Android.OS.Environment.DirectoryPictures);

                    var contentResolver = Platform.CurrentActivity.ContentResolver;
                    var uri = contentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, values);

                    if (uri != null)
                    {
                        using (var inputStream = File.OpenRead(imagePath))
                        using (var outputStream = contentResolver.OpenOutputStream(uri))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                    //File.Delete(imagePath);
                }
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    var picturesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    var destinationPath = Path.Combine(picturesDirectory, Path.GetFileName(imagePath));
                    File.Copy(imagePath, destinationPath, true);
                }
            }
        }

        private async Task CheckShowReviewKey()
        {
            if (Preferences.Get(App.ShowReviewKey, true))
            {
                _navigationService.PopModalAsync();
                await _navigationService.NavigateToContextPageAsync();
            }
            else
            {
                _navigationService.PopModalAsync();
                await _navigationService.NavigateToResultPageAsync();
            }
        }
    }
}

