using System;
namespace AICalories.Services
{
    public interface ICameraService
    {
        Task<Stream> TakePhotoAsync();
        void EnableTorch();
        void DisableTorch();
        bool IsTorchEnabled();
    }
}

