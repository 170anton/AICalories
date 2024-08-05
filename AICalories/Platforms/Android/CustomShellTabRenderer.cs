using AICalories;
using Android.Content;
using Android.Graphics;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

//[assembly: ExportRenderer(typeof(AppShell), typeof(AICalories.Platforms.Android.CustomShellTabRenderer))]
namespace AICalories.Platforms.Android
{
	public class CustomShellTabRenderer : ShellRenderer
    {
        public CustomShellTabRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Shell.CurrentItem))
            {
                UpdateToolbar();
            }
        }

        private void UpdateToolbar()
        {
            var activity = Platform.CurrentActivity;
            var toolbar = activity.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(activity.Resources.GetIdentifier("toolbar", "id", activity.PackageName));

            //var toolbar = activity.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                // Customize the toolbar appearance here
                //toolbar.SetBackgroundColor(Android.Graphics.Color.Rgb(0, 150, 136)); // Change the bar color
                //toolbar.SetTitleTextColor(Android.Graphics.Color.White); // Change the text color

                // Hide the title
                activity.ActionBar.Title = string.Empty;
            }
        }
    }
}

