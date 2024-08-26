using System;
using Camera.MAUI;

namespace AICalories.Services
{
    public class CameraService : ICameraService
    {
        private readonly CameraView _cameraView;

        public CameraService(CameraView cameraView)
        {
            _cameraView = cameraView;
        }

        public async Task<Stream> TakePhotoAsync()
        {
            return await _cameraView.TakePhotoAsync();
        }

        public void EnableTorch()
        {
            _cameraView.TorchEnabled = true;
            _cameraView.FlashMode = FlashMode.Enabled;
        }

        public void DisableTorch()
        {
            _cameraView.TorchEnabled = false;
            _cameraView.FlashMode = FlashMode.Disabled;
        }

        public bool IsTorchEnabled()
        {
            return _cameraView.TorchEnabled;
        }

        public async Task DisposeCamera()
        {
            await _cameraView.StopCameraAsync();
            
        }
    }

}

