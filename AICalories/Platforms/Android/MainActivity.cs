using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace AICalories;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
            base.OnCreate(savedInstanceState);

            // Set the activity to fullscreen mode
            //Window.SetFlags(WindowManagerFlags.ForceNotFullscreen, WindowManagerFlags.ForceNotFullscreen);


            //// Prevent layout changes when the soft input mode changes
            //Window.SetSoftInputMode(SoftInput.AdjustResize);

            //// Optional: Keep the screen on while the activity is visible
            //Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
        }

}
