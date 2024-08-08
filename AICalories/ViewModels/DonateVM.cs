using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using AICalories.Models;
using Newtonsoft.Json;

namespace AICalories.ViewModels
{
    public class DonateVM : INotifyPropertyChanged
    {
        private decimal _amount;

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public ICommand DonateCommand { get; }

        public DonateVM()
        {
            DonateCommand = new Command(OnDonate);
        }

        private async void OnDonate()
        {
            var donation = new Donation { Amount = this.Amount };

            // Replace with your API URL and logic
            var url = "https://your-api-url.com/donate";

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(donation);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful donation
                }
                else
                {
                    // Handle failed donation
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}