using System;
using AICalories.Interfaces;

namespace AICalories.Models
{
	public class ImageInfo : IImageInfo
	{

        public ImageSource Image { get; set; }
        public string MealType { get; set; }
        public List<string> Hints { get; set; }

        public ImageInfo()
		{
		}
	}
}

