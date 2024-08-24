using System;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Services;
using Android.Content;
using Android.Provider;
using Java.Util.Streams;

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

            CaptureCommand = new Command(async () => await OnCaptureButtonClicked());
            GalleryCommand = new Command(async () => await OnGalleryButtonClicked());
            ToggleTorchCommand = new Command(OnToggleTorchButtonClicked);
        }



        private async Task OnCaptureButtonClicked() //todo add try catch
        {
            var stream = await _cameraService.TakePhotoAsync();

            if (stream == null)
            {
                return; // Handle no photo taken
            }

            var imageName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            var imagePath = Path.Combine(FileSystem.CacheDirectory, imageName);

            await using (var fileStream = File.Create(imagePath))
            {
                await stream.CopyToAsync(fileStream);
            }
            SaveToGallery(imagePath);

            SetImage(imagePath);

            _navigationService.PopModalAsync();
            await _navigationService.NavigateToContextPageAsync();
        }

        private async Task OnGalleryButtonClicked()
        {
            var image = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Please select an image"
            });

            if (image == null)
            {
                // Handle the case when no image was selected
                return;
            }

            // Copy the selected image to the local cache
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

            _navigationService.PopModalAsync();
            await _navigationService.NavigateToContextPageAsync();
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

        private void SaveToGallery(string imagePath)
        {
            if (Preferences.Get(App.SaveToGalleryKey, true))
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


        public async void SetImage(string imagePath)
        {
            _imageInfo.ImagePath = imagePath;
        }
    }
}

