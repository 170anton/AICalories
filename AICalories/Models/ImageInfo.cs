using System;
using AICalories.Interfaces;

namespace AICalories.Models
{
	public class ImageInfo : IImageInfo
	{
        private string userInfo;

        public string ImagePath { get; set; }
        public string MealType { get; set; }//"This meal includes following ingredients information provided by the user: "
        public string UserInfo { get => "User provided additional information: " + " '" + userInfo + "' ";
                                set => userInfo = value; }
        //public List<string> Hints { get; set; }

        public ImageInfo()
		{
		}

        public void Clear()
        {
            ImagePath = null;
            MealType = null;
            userInfo = null;
        }
	}
}

