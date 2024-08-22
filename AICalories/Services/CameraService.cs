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
        }

        public void DisableTorch()
        {
            _cameraView.TorchEnabled = false;
        }

        public bool IsTorchEnabled()
        {
            return _cameraView.TorchEnabled;
        }
    }

}

