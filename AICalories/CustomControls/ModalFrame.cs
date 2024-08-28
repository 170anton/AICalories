using System;
namespace AICalories.CustomControls
{
    public class ModalFrame : Frame
    {
        public ModalFrame()
        {
            // Set default properties for the Frame
            BackgroundColor = Colors.White;
            Padding = new Thickness(20);
            HasShadow = true;
            CornerRadius = 10;
            IsVisible = false; // Initially hidden
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;

            // You can also define default content here, or leave it for customization
            var contentLayout = new StackLayout
            {
                Children =
                {
                    new Label { Text = "Default Option 1" },
                    new Label { Text = "Default Option 2" },
                    new Label { Text = "Default Option 3" }
                }
            };

            Content = contentLayout;
        }

        // Optionally, you can add methods to control the frame visibility
        public void Show()
        {
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }
    }
}

