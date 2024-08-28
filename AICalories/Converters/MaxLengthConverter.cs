using System;
using System.Globalization;

namespace AICalories.Converters
{
    public class MaxLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Check if the string length exceeds the maximum length
                if (stringValue.Length > 5)
                {
                    // Return the last 5 characters
                    return stringValue.Substring(stringValue.Length - 5, 5);
                }
                return stringValue; // Return the full string if it's 5 characters or less
            }

            return value; // Return the original value if it's not a string
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Optional: if you want to support converting back
            return value;
        }
    }
}

