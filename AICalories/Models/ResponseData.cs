using System;

namespace AICalories.Models
{
	public class ResponseData
    {
        private string dishName;

        public string MealName { get => dishName;
            set => dishName = char.ToUpper(value[0]) + value.Substring(1); }
        public int Calories { get; set; }
        public string TotalResultJSON { get; set; }
	}
}

