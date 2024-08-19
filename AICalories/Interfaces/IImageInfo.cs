using System;
namespace AICalories.Interfaces
{
	public interface IImageInfo
	{
		public ImageSource Image { get; set; }
		public string MealType { get; set; }
		public List<string> Hints { get; set; }
	}
}

