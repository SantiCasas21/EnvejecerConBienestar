namespace EnvejecerConBienestar;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registro de rutas de navegación
        Routing.RegisterRoute(nameof(Views.HomePage), typeof(Views.HomePage));
        Routing.RegisterRoute(nameof(Views.MedicamentosPage), typeof(Views.MedicamentosPage));
    }
}
