using System;
using AICalories.Interfaces;

namespace AICalories.ViewModels
{
	public class TakeImageVM
    {
        private IImageInfo _imageInfo;
        public TakeImageVM(IImageInfo imageInfo)
        {
            _imageInfo = imageInfo;
        }



        public async void SetImage(ImageSource image)
        {
            _imageInfo.Image = image;
        }
    }
}

