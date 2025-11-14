using Microsoft.Extensions.Logging;
using Student_Partoza_IT13.Services; // add AuthService namespace

namespace Student_Partoza_IT13
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<AuthService>(); // add placeholder auth service
            builder.Services.AddSingleton<StudentService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
