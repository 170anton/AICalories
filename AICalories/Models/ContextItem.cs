using System;
using AICalories.Interfaces;
using SQLite;

namespace AICalories.Models
{
	public class ContextItem : IContextItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
    }
}

