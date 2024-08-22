using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Services;
using AICalories.ViewModels;
using AICalories.Views;
using Camera.MAUI;
using Microsoft.Extensions.Logging;

namespace AICalories;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCameraView()
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


        // Register the view model service
        builder.Services.AddSingleton<IViewModelService, ViewModelService>();
        builder.Services.AddSingleton<IImageInfo, ImageInfo>();  //todo try Transient
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IAlertService, AlertService>();
        //builder.Services.AddSingleton<ICameraService, CameraService>();
        builder.Services.AddTransient<ICameraService, CameraService>();

        builder.Services.AddSingleton<ViewModelLocator>();
        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<AppSettingsVM>();
        builder.Services.AddTransient<MainVM>();
        builder.Services.AddTransient<ContextVM>();
        builder.Services.AddTransient<TakeImageVM>();
        builder.Services.AddTransient<ResultVM>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<TakeImagePage>();
        builder.Services.AddTransient<ContextPage>();
        builder.Services.AddTransient<ResultPage>();




#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
