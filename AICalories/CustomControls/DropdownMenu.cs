using System;
using static System.Net.Mime.MediaTypeNames;

namespace AICalories.CustomControls
{
    public class DropdownMenu : StackLayout
    {
        private ContentView _modalBackground;
        private Frame _modalFrame;

        public DropdownMenu()
        {
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            var overflowButton = new Button
            {
                Text = "⋮",
                BackgroundColor = Colors.Transparent,
                BorderColor = Colors.Transparent,
                TextColor = Colors.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                //HorizontalOptions = LayoutOptions.Start,
                //VerticalOptions = LayoutOptions.Start,
            };

            _modalBackground = new ContentView
            {
                IsVisible = false,
                BackgroundColor = new Color(0, 0, 0, 0.5f), // Semi-transparent black background
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = CreateModalFrame()
            };

            overflowButton.Clicked += OnOverflowButtonClicked;


            this.Children.Add(overflowButton);
            this.Children.Add(_modalBackground);


            var menuItem = new Button
            {
                Text = "Test",
                Padding = 10,
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            //_modalFrame.c.Add(menuItem);
        }

        private void OnOverflowButtonClicked(object sender, EventArgs e)
        {
            _modalBackground.IsVisible = !_modalBackground.IsVisible;
        }

        public void AddMenuItem(string text, EventHandler clickHandler)
        {
            var menuItem = new Button
            {
                Text = text,
                Padding = 10,
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            menuItem.Clicked += (s, e) =>
            {
                clickHandler(s, e);
                _modalBackground.IsVisible = false; // Hide the modal after selection
            };

            if (_modalFrame.Content is StackLayout stackLayout)
            {
                stackLayout.Children.Add(menuItem);
            }
        }
        private Frame CreateModalFrame()
        {
            _modalFrame = new Frame
            {
                BackgroundColor = Colors.White,
                Padding = 15,
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 250,
                HeightRequest = 150
            };

            var stackLayout = new StackLayout
            {
                Spacing = 10
            };

            _modalFrame.Content = stackLayout;

            return _modalFrame;
        }
    }
}

