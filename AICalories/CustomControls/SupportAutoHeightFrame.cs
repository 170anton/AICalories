using System;
namespace AICalories.CustomControls
{
    public class CustomAutoHeightFrame : Frame
    {
        public static readonly BindableProperty AutoHeightProperty =
            BindableProperty.Create(nameof(AutoHeight), typeof(bool), typeof(CustomAutoHeightFrame), false, propertyChanged: OnAutoHeightChanged);

        public bool AutoHeight
        {
            get => (bool)GetValue(AutoHeightProperty);
            set => SetValue(AutoHeightProperty, value);
        }

        private static void OnAutoHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var frame = (CustomAutoHeightFrame)bindable;
            if (frame.AutoHeight)
            {
                // Logic to update frame size based on content
                frame.InvalidateMeasure();
            }
        }
    }
}

