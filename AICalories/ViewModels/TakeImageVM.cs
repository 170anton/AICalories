using System;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Services;

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

                await SaveToGallery(imagePath);

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
                await _navigationService.PopModalAsync();
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

        #region Save Image to Gallery

        private async Task<bool> SaveToGallery(string imagePath)
        {

            if (Preferences.Get(App.SaveToGalleryKey, false))
            {
#if ANDROID
                return await SaveImageToGalleryAndroid(imagePath);
#elif IOS
                return await SaveImageToGalleryiOS(imagePath);
#else
                // Handle other platforms or return false
                return false;
#endif
            }
            return false;
            //if (DeviceInfo.Platform == DevicePlatform.Android)
            //{
            //    var fileName = Path.GetFileName(imagePath);
            //    var mimeType = "image/jpeg";
            //    var values = new Android.Content.ContentValues();
            //    values.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, fileName);
            //    values.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, mimeType);
            //    values.Put(Android.Provider.MediaStore.Images.ImageColumns.RelativePath, Android.OS.Environment.DirectoryPictures);

            //    var contentResolver = Platform.CurrentActivity.ContentResolver;
            //    var uri = contentResolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, values);

            //    if (uri != null)
            //    {
            //        using (var inputStream = File.OpenRead(imagePath))
            //        using (var outputStream = contentResolver.OpenOutputStream(uri))
            //        {
            //            inputStream.CopyTo(outputStream);
            //        }
            //    }
            //    //File.Delete(imagePath);
            //}
            //else if (DeviceInfo.Platform == DevicePlatform.iOS)
            //{
            //    var picturesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            //    var destinationPath = Path.Combine(picturesDirectory, Path.GetFileName(imagePath));
            //    File.Copy(imagePath, destinationPath, true);
            //}

        }

        public async Task<bool> SaveImageToGalleryiOS(string imagePath)
        {
            try
            {
                var picturesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                var destinationPath = Path.Combine(picturesDirectory, Path.GetFileName(imagePath));
                File.Copy(imagePath, destinationPath, true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image to gallery: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SaveImageToGalleryAndroid(string imagePath)
        {
            try
            {
                // Check and request permissions if necessary
                if (DeviceInfo.Version.Major < 10)
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                        if (status != PermissionStatus.Granted)
                            return false;
                    }
                }

                // Prepare file metadata
                var fileName = Path.GetFileName(imagePath);
                var mimeType = "image/jpeg"; // Adjust if necessary
                var values = new Android.Content.ContentValues();
                values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.DisplayName, fileName);
                values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.MimeType, mimeType);
                values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.DateAdded, Java.Lang.JavaSystem.CurrentTimeMillis() / 1000);
                values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.DateTaken, Java.Lang.JavaSystem.CurrentTimeMillis());

                if (DeviceInfo.Version.Major >= 10)
                {
                    values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.RelativePath, Android.OS.Environment.DirectoryPictures);
                }
                else
                {
                    var filePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                    values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data, Path.Combine(filePath, fileName));
                }

                var uri = Platform.CurrentActivity.ContentResolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, values);

                using (var inputStream = File.OpenRead(imagePath))
                using (var outputStream = Platform.CurrentActivity.ContentResolver.OpenOutputStream(uri))
                {
                    await inputStream.CopyToAsync(outputStream);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image to gallery: {ex.Message}");
                return false;
            }
        }

        #endregion

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

