using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Services;
using AICalories.ViewModels;
using AICalories.Views;
using Android.Gms.Ads;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Licensing;

namespace AICalories;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
        SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF1cWWhAYVF2WmFZfVpgcV9HY1ZURmYuP1ZhSXxXdkxiWn9bcXdQRGZdV0w=");

        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            //.UseMauiEssentials()
            .ConfigureSyncfusionCore()
            .UseMauiCommunityToolkitCamera()
            
            //.ConfigureMauiHandlers(handlers =>
            //{
            //	handlers.AddHandler(typeof(CutImage), typeof(SKCanvasViewHandler));
            ////})
            //.ConfigureMauiHandlers(handlers =>
            //{
            //	handlers.AddHandler(typeof(SKCanvasView), typeof(SKCanvasViewHandler));
            //})
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Ubuntu-Regular.ttf", "UbuntuRegular");
            });



        MobileAds.Initialize(Android.App.Application.Context);

        MobileAds.RequestConfiguration = new RequestConfiguration.Builder()
            .SetTestDeviceIds(new List<string> { "8C15E615345B4618D0BE650BE252E0CC" }) // Replace with your actual device ID
            .Build();

#if ANDROID
        builder.Services.AddSingleton<IKeyboardHelper, Platforms.Android.KeyboardHelper>();
        //#elif IOS
        //        builder.Services.AddSingleton<IKeyboardHelper, AICalories.Platforms.iOS.KeyboardHelper>();
#endif


        // Register the view model service
        builder.Services.AddSingleton<IViewModelService, ViewModelService>();
        builder.Services.AddSingleton<IImageInfo, ImageInfo>();  //todo try Transient
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IAlertService, AlertService>();
        builder.Services.AddTransient<ICameraService, CameraService>();

        builder.Services.AddSingleton<ViewModelLocator>();
        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<MainVM>();
        builder.Services.AddTransient<AppSettingsVM>();
        builder.Services.AddTransient<HistoryVM>();
        builder.Services.AddTransient<TakeImageVM>();
        builder.Services.AddTransient<ContextVM>();
        builder.Services.AddTransient<ResultVM>();
        builder.Services.AddTransient<MealInfoVM>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AppSettingsPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<TakeImagePage>();
        builder.Services.AddTransient<ContextPage>();
        builder.Services.AddTransient<ResultPage>();
        builder.Services.AddTransient<MealInfoPage>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
