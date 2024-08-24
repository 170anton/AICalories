﻿using System;
namespace AICalories.Interfaces
{
	public interface IImageInfo
	{
		public string ImagePath { get; set; }
		public string MealType { get; set; }
        public string UserInfo { get; set; }
		void Clear();
    }
}

