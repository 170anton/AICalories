using System;
namespace AICalories.Interfaces
{
	public interface IContextItem
	{
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
    }
}

