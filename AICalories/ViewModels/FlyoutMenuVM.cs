using System;
using System.Collections.ObjectModel;
using AICalories.Models;

namespace AICalories.ViewModels
{
	public class FlyoutMenuVM
	{
        public ObservableCollection<FlyoutMenuItem> MenuItems { get; set; }

        public FlyoutMenuVM()
		{
            MenuItems = new ObservableCollection<FlyoutMenuItem>(new[]
                {
                    new FlyoutMenuItem { Id = 0, Title = "FAQ" },
                    new FlyoutMenuItem { Id = 1, Title = "Support" },
                    new FlyoutMenuItem { Id = 2, Title = "About" },
                    new FlyoutMenuItem { Id = 3, Title = "-" },
                    new FlyoutMenuItem { Id = 4, Title = "-" }
                });
        }
	}
}

