using System;
using Microsoft.Maui.Controls;

namespace AICalories.Models
{
	public class HistoryItemFrame : Frame
	{
        //public ContentPage _contentPage { get; protected set; }
        public HistoryItemFrame()
		{
            //_contentPage = contentPage;

            Padding = new Thickness(0);
            Margin = new Thickness(20, 5, 0, 0);
            BorderColor = (Color)Application.Current.Resources["Primary"];
            HeightRequest = 50;
            CornerRadius = 10;


            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnFrameTapped;
            this.GestureRecognizers.Add(tapGestureRecognizer);
        }
        public async void OnFrameTapped(object sender, EventArgs e)
        {
            // Animate the Frame to give a click effect
            await this.ScaleTo(0.98, 100, Easing.SpringOut); // Scale down
            await this.ScaleTo(1, 100, Easing.SpringIn);   // Scale back to normal

            // Perform other actions here, like navigation or logic
            //await Application.Current.MainPage.DisplayAlert("Clicked!", "You clicked the frame.", "OK");
            //try
            //{
            //    bool delete = await Application.Current.MainPage.DisplayAlert("Delete Item", "Do you want to delete this item?", "Yes", "No");
            //        if (delete)
            //        {
            //            if (_viewModel != null)
            //            {
            //                _viewModel.DeleteItemCommand.Execute(selectedItem);
            //            }
            //        }
            //    }
            //}
        }
    }
}

