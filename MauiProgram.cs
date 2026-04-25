using Microsoft.Extensions.Logging;
using EnvejecerConBienestar.ViewModels;
using EnvejecerConBienestar.Views;

namespace EnvejecerConBienestar;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
            });

        // Registrar AppShell
        builder.Services.AddSingleton<AppShell>();

        // Registrar ViewModels
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<MedicamentosViewModel>();

        // Registrar Views
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<MedicamentosPage>();
        builder.Services.AddSingleton<JuegosPage>();
        builder.Services.AddSingleton<ContactosPage>();

#if DEBUG
        //builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
