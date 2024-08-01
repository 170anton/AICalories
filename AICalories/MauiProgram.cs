using AICalories.Models;
using Microsoft.Extensions.Logging;
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
   //         .ConfigureMauiHandlers(handlers =>
			//{
			//	handlers.AddHandler(typeof(CutImage), typeof(SKCanvasViewHandler));
			//})
			.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler(typeof(SKCanvasView), typeof(SKCanvasViewHandler));
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Ubuntu-Regular.ttf", "UbuntuRegular");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
