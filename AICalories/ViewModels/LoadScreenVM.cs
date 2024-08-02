using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AICalories.ViewModels
{
	public class LoadScreenVM : INotifyPropertyChanged
    {
        private bool isRefreshing;
        private string aIResponse;

        public string AIResponse
        {
            get => aIResponse;
            set
            {
                if (aIResponse != value)
                {
                    aIResponse = value;
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

        public LoadScreenVM()
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

