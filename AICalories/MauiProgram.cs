using AICalories.DI;
using AICalories.Models;
using AICalories.ViewModels;
using AICalories.Views;
using Microsoft.Extensions.Logging;
using Plugin.Media;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui.Handlers;

namespace AICalories;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
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

        // Register view models
        builder.Services.AddTransient<MainVM>();
        builder.Services.AddTransient<ContextVM>();
        builder.Services.AddTransient<AppSettingsVM>();

        builder.Services.AddSingleton<ViewModelLocator>();
        builder.Services.AddSingleton<AppShell>();



#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
