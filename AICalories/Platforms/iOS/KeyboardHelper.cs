using System;
using AICalories.Interfaces;
using AICalories.Platforms.iOS;
using UIKit;

[assembly: Dependency(typeof(KeyboardHelper))]
namespace AICalories.Platforms.iOS
{
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            //UIApplication.SharedApplication.KeyWindow.EndEditing(true);

            var window = UIApplication.SharedApplication?
                                      .ConnectedScenes
                                      .OfType<UIWindowScene>()
                                      .SelectMany(scene => scene.Windows)
                                      .FirstOrDefault(window => window.IsKeyWindow);

            window?.EndEditing(true);
        }
    }
}

