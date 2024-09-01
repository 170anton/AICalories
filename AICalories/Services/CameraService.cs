using System;
using CommunityToolkit.Maui.Views;

namespace AICalories.Services
{
    public class CameraService : ICameraService
    {
        private readonly CameraView _cameraView;

        public CameraService(CameraView cameraView)
        {
            _cameraView = cameraView;
        }

        //public async Task<Stream> TakePhotoAsync()
        //{
        //    return await _cameraView.CaptureImage(new CancellationToken());
        //}

        public void EnableTorch()
        {
            _cameraView.IsTorchOn = true;
            _cameraView.CameraFlashMode = CommunityToolkit.Maui.Core.Primitives.CameraFlashMode.On;
        }

        public void DisableTorch()
        {
            _cameraView.IsTorchOn = false;
            _cameraView.CameraFlashMode = CommunityToolkit.Maui.Core.Primitives.CameraFlashMode.Off;
        }

        public bool IsTorchEnabled()
        {
            return _cameraView.IsTorchOn;
        }

        public void ToggleTorch()
        {
            _cameraView.IsTorchOn = !_cameraView.IsTorchOn;

            if (_cameraView.IsTorchOn)
                _cameraView.CameraFlashMode = CommunityToolkit.Maui.Core.Primitives.CameraFlashMode.On;
            else
                _cameraView.CameraFlashMode = CommunityToolkit.Maui.Core.Primitives.CameraFlashMode.Off;
        }

        public void DisposeCamera()
        {
            _cameraView.StopCameraPreview();
        }
    }

}

