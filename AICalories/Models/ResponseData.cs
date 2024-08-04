using System;

namespace AICalories.Models
{
	public class ResponseData
    {
        private string dishName;

        public string DishName { get => dishName;
            set => dishName = char.ToUpper(value[0]) + value.Substring(1); }
        public int Calories { get; set; }

        public ResponseData()
		{
		}
	}
}

