using Enharmonica.Services;
using Enharmonica.ViewModels;
using Enharmonica.Views;
using Microsoft.Extensions.Logging;

namespace Enharmonica
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddTransient<ScalesPage>();
            builder.Services.AddTransient<ScalesPageViewModel>();

            builder.Services.AddTransient<ChordsPage>();
            builder.Services.AddTransient<ChordsPageViewModel>();

            builder.Services.AddTransient<MetronomePage>();
            builder.Services.AddTransient<MetronomePageViewModel>();

            builder.Services.AddTransient<PitchDetectionPage>();
            builder.Services.AddTransient<PitchDetectionPageViewModel>();
#endif
//            // Configure the window size for Windows platform
//            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
//            {
//#if WINDOWS
//                var nativeWindow = handler.PlatformView;
//                nativeWindow.Width = 375; // Set window width
//                nativeWindow.Height = 667; // Set window height
//#endif
//            });
            return builder.Build();
        }
    }
}
