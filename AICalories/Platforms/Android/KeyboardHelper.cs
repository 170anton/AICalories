using System;
using AICalories.Interfaces;
using AICalories.Platforms.Android;
using Android.Content;
using Android.Views.InputMethods;

[assembly: Dependency(typeof(KeyboardHelper))]
namespace AICalories.Platforms.Android
{
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            var activity = Platform.CurrentActivity;
            var inputMethodManager = activity?.GetSystemService(Context.InputMethodService) as InputMethodManager;

            var token = activity?.CurrentFocus?.WindowToken;
            if (token != null)
            {
                inputMethodManager?.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
            }
        }
    }
}

