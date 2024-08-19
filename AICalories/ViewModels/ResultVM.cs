using System;
using AICalories.Interfaces;

namespace AICalories.ViewModels
{
	public class ResultVM
    {
        private IImageInfo _imageInfo;
        public ResultVM(IImageInfo imageInfo)
        {
            _imageInfo = imageInfo;
        }
	}
}

