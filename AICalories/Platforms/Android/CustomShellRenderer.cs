using System.ComponentModel;
using AICalories;
using Android.Content;
using Android.Views;
using AndroidX.DrawerLayout.Widget;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;

[assembly: ExportRenderer(typeof(AppShell), typeof(AICalories.Platforms.Android.CustomShellRenderer))]
namespace AICalories.Platforms.Android
{
    public class CustomShellRenderer : ShellRenderer
    {
        public CustomShellRenderer(Context context) : base(context)
        {
        }

        protected override IShellFlyoutContentRenderer CreateShellFlyoutContentRenderer()
        {
            var renderer = base.CreateShellFlyoutContentRenderer();
            var layoutParams = new DrawerLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            {
                Gravity = (int)GravityFlags.End
            };
            renderer.AndroidView.LayoutParameters = layoutParams;
            return renderer;
        }
    }
}