using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Services;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;

namespace AICalories.ViewModels
{
	public class TakeImageVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        private readonly ICameraService _cameraService;
        private CameraInfo _selectedCamera;
        private Size _selectedResolution;
        private CameraFlashMode _flashMode;

        private IImageInfo _imageInfo;

        public ICommand CaptureCommand { get; }
        public ICommand GalleryCommand { get; }
        public ICommand ToggleTorchCommand { get; }

        #region Properties

        public CameraInfo SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                _selectedCamera = value;
                OnPropertyChanged();
            }
        }

        public Size SelectedResolution
        {
            get => _selectedResolution;
            set
            {
                _selectedResolution = value;
                OnPropertyChanged();
            }
        }

        public CameraFlashMode FlashMode
        {
            get => _flashMode;
            set
            {
                _flashMode = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public TakeImageVM(IViewModelService viewModelService, IImageInfo imageInfo, ICameraService cameraService,
            INavigationService navigationService, IAlertService alertService)
        {
            _viewModelService = viewModelService;
            _viewModelService.TakeImageVM = this;
            _navigationService = navigationService;
            _alertService = alertService;
            //_cameraService = cameraService;


            CaptureCommand = new Command<string>(async (imagePath) => await OnCaptureButtonClicked(imagePath));
            GalleryCommand = new Command(async () => await OnGalleryButtonClicked());
            ToggleTorchCommand = new Command(async () => await OnToggleTorchButtonClickedAsync());

            _imageInfo = imageInfo;
            _imageInfo.Clear();
        }

        public async Task SetImage(string imagePath)
        {
            try
            {
                _imageInfo.ImagePath = imagePath;
            }
            catch (Exception)
            {
                _alertService.ShowError("Failed to load image");
                await _navigationService.PopModalAsync();
            }
        }

        private async Task OnCaptureButtonClicked(string imagePath)
        {
            try
            {
                //string imagePath = await SaveImage(stream);

                //await SaveToGallery(imagePath);

                await SetImage(imagePath);

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

                var image = await MediaPicker.PickPhotoAsync();

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
            catch (Exception ex)
            {
                await _navigationService.PopModalAsync();
                _alertService.ShowError("Failed to load image");
                Console.WriteLine(ex.Message);
            }

        }


        private async Task OnToggleTorchButtonClickedAsync()
        {
            var IsTorchSupported = SelectedCamera.IsFlashSupported;
            FlashMode = CameraFlashMode.On;
            await Flashlight.TurnOnAsync();
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

//        #region Save Image to Gallery

//        private async Task<bool> SaveToGallery(string imagePath)
//        {

//            if (Preferences.Get(App.SaveToGalleryKey, false))
//            {
//#if ANDROID
//                return await SaveImageToGalleryAndroid(imagePath);
//#elif IOS
//                return await SaveImageToGalleryiOS(imagePath);
//#else
//                // Handle other platforms or return false
//                return false;
//#endif
//            }
//            return false;
//            //if (DeviceInfo.Platform == DevicePlatform.Android)
//            //{
//            //    var fileName = Path.GetFileName(imagePath);
//            //    var mimeType = "image/jpeg";
//            //    var values = new Android.Content.ContentValues();
//            //    values.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, fileName);
//            //    values.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, mimeType);
//            //    values.Put(Android.Provider.MediaStore.Images.ImageColumns.RelativePath, Android.OS.Environment.DirectoryPictures);

//            //    var contentResolver = Platform.CurrentActivity.ContentResolver;
//            //    var uri = contentResolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, values);

//            //    if (uri != null)
//            //    {
//            //        using (var inputStream = File.OpenRead(imagePath))
//            //        using (var outputStream = contentResolver.OpenOutputStream(uri))
//            //        {
//            //            inputStream.CopyTo(outputStream);
//            //        }
//            //    }
//            //    //File.Delete(imagePath);
//            //}
//            //else if (DeviceInfo.Platform == DevicePlatform.iOS)
//            //{
//            //    var picturesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
//            //    var destinationPath = Path.Combine(picturesDirectory, Path.GetFileName(imagePath));
//            //    File.Copy(imagePath, destinationPath, true);
//            //}

//        }

//        public async Task<bool> SaveImageToGalleryiOS(string imagePath)
//        {
//            try
//            {
//                var picturesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
//                var destinationPath = Path.Combine(picturesDirectory, Path.GetFileName(imagePath));
//                File.Copy(imagePath, destinationPath, true);

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error saving image to gallery: {ex.Message}");
//                return false;
//            }
//        }


//        public async Task<bool> SaveImageToGalleryAndroid(string imagePath)
//        {
//            try
//            {
//                // Check and request permissions if necessary
//                if (DeviceInfo.Version.Major < 10)
//                {
//                    var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
//                    if (status != PermissionStatus.Granted)
//                    {
//                        status = await Permissions.RequestAsync<Permissions.StorageWrite>();
//                        if (status != PermissionStatus.Granted)
//                            return false;
//                    }
//                }

//                // Prepare file metadata
//                var fileName = Path.GetFileName(imagePath);
//                var mimeType = "image/jpeg"; // Adjust if necessary
//                var values = new ContentValues();
//                values.Put(MediaStore.Images.Media.InterfaceConsts.DisplayName, fileName);
//                values.Put(MediaStore.Images.Media.InterfaceConsts.MimeType, mimeType);
//                values.Put(MediaStore.Images.Media.InterfaceConsts.DateAdded, Java.Lang.JavaSystem.CurrentTimeMillis() / 1000);
//                values.Put(MediaStore.Images.Media.InterfaceConsts.DateTaken, Java.Lang.JavaSystem.CurrentTimeMillis());

//                if (DeviceInfo.Version.Major >= 10)
//                {
//                    values.Put(MediaStore.Images.Media.InterfaceConsts.RelativePath, Android.OS.Environment.DirectoryPictures);
//                }
//                else
//                {
//                    var filePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
//                    values.Put(MediaStore.Images.Media.InterfaceConsts.Data, Path.Combine(filePath, fileName));
//                }

//                var uri = Platform.CurrentActivity.ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, values);

//                using (var inputStream = File.OpenRead(imagePath))
//                using (var outputStream = Platform.CurrentActivity.ContentResolver.OpenOutputStream(uri))
//                {
//                    await inputStream.CopyToAsync(outputStream);
//                }

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error saving image to gallery: {ex.Message}");
//                return false;
//            }
//        }

//        #endregion

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

