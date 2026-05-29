using Microsoft.Extensions.Logging;
using EnvejecerConBienestar.ViewModels;
using EnvejecerConBienestar.Views;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;

namespace EnvejecerConBienestar;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
                fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");
            });

        // Registrar AppShell
        builder.Services.AddSingleton<AppShell>();

        // Registrar Servicios
        builder.Services.AddSingleton<Services.DatabaseService>();
        builder.Services.AddSingleton<Services.AlarmService>();
        builder.Services.AddSingleton<Services.ContactService>();
        builder.Services.AddSingleton<Services.ReportService>();

        // Registrar ViewModels
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<MedicamentosViewModel>();
        builder.Services.AddSingleton<JuegosViewModel>();
        builder.Services.AddSingleton<ContactosViewModel>();
        builder.Services.AddTransient<ContactoDetailViewModel>();
        builder.Services.AddTransient<MedicamentoDetailViewModel>();
        builder.Services.AddTransient<PerfilViewModel>();

        // Registrar Views
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<MedicamentosPage>();
        builder.Services.AddTransient<JuegosPage>();
        builder.Services.AddTransient<ContactosPage>();
        builder.Services.AddTransient<ContactoDetailPage>();
        builder.Services.AddTransient<MedicamentoDetailPage>();
        builder.Services.AddTransient<BuscarParesPage>();
        builder.Services.AddTransient<TriviaPage>();
        builder.Services.AddSingleton<PerfilPage>();

#if DEBUG
        //builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
