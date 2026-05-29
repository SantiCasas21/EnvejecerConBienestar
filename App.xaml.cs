using EnvejecerConBienestar.Services;

namespace EnvejecerConBienestar;

public partial class App : Application
{
    public App(AppShell shell)
    {
        InitializeComponent();
        MainPage = shell;

        Task.Run(async () =>
        {
            await Task.Delay(500);
            await AlarmService.InicializarCanal();
        });
    }
}