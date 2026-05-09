using Microsoft.Extensions.Logging;
using EnvejecerConBienestar.ViewModels;
using EnvejecerConBienestar.Views;
using CommunityToolkit.Maui;

namespace EnvejecerConBienestar;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
            });

        // Registrar AppShell
        builder.Services.AddSingleton<AppShell>();

        // Registrar Servicios
        builder.Services.AddSingleton<Services.DatabaseService>();
        builder.Services.AddSingleton<Services.AlarmService>();
        builder.Services.AddSingleton<Services.ContactService>();

        // Registrar ViewModels
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<MedicamentosViewModel>();
        builder.Services.AddSingleton<JuegosViewModel>();
        builder.Services.AddSingleton<ContactosViewModel>();
        builder.Services.AddTransient<ContactoDetailViewModel>();
        builder.Services.AddTransient<MedicamentoDetailViewModel>();

        // Registrar Views
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<MedicamentosPage>();
        builder.Services.AddTransient<JuegosPage>();
        builder.Services.AddTransient<ContactosPage>();
        builder.Services.AddTransient<ContactoDetailPage>();
        builder.Services.AddTransient<MedicamentoDetailPage>();

#if DEBUG
        //builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
