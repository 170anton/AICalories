using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AICalories.ViewModels
{
	public class LoadingScreenVM : INotifyPropertyChanged
    {
        private bool isRefreshing;
        private string dishName;
        private string calories;
        private string totalResultJSON;

        public string DishName
        {
            get => dishName;
            set
            {
                if (dishName != value)
                {

                    dishName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Calories
        {
            get => calories;
            set
            {
                if (calories != value)
                {
                    calories = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TotalResultJSON
        {
            get => totalResultJSON;
            set
            {
                if (totalResultJSON != value)
                {
                    totalResultJSON = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }

        public LoadingScreenVM()
		{
            isRefreshing = true;
		}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

